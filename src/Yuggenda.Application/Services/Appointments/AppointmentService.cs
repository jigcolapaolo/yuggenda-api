using Yuggenda.Application.Abstractions.Authentication;
using Yuggenda.Application.Abstractions.Persistence;
using Yuggenda.Application.DTOs.Appointments;
using Yuggenda.Application.Services.Availabilities;
using Yuggenda.Domain.Entities;
using Yuggenda.Domain.Enums;
using Yuggenda.Domain.Exceptions;

namespace Yuggenda.Application.Services.Appointments;

public class AppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUserRepository _userRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IBusinessRepository _businessRepository;
    private readonly IBusinessMemberRepository _businessMemberRepository;
    private readonly AvailabilityCalculationService _availabilityCalculationService;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        ICustomerRepository customerRepository,
        IUserRepository userRepository,
        IServiceRepository serviceRepository,
        IBusinessRepository businessRepository,
        IBusinessMemberRepository businessMemberRepository,
        AvailabilityCalculationService availabilityCalculationService,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _customerRepository = customerRepository;
        _userRepository = userRepository;
        _serviceRepository = serviceRepository;
        _businessRepository = businessRepository;
        _businessMemberRepository = businessMemberRepository;
        _availabilityCalculationService = availabilityCalculationService;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<AppointmentResponse> CreateAsync(
        Guid businessId,
        CreateAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        var business = await _businessRepository.GetByIdAsync(
            businessId, cancellationToken);

        if (business is null)
            throw new KeyNotFoundException("Business not found.");

        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(business.Timezone);

        var localStartTime = TimeZoneInfo.ConvertTime(
            request.StartTime, timeZone).DateTime;

        var date = DateOnly.FromDateTime(localStartTime);

        var availableSlots = await _availabilityCalculationService
            .GetAvailableSlotsForServiceAsync(
                businessId,
                request.ServiceId,
                date,
                cancellationToken,
                request.BusinessMemberId);

        var requestedSlot = availableSlots.FirstOrDefault(
            slot => slot.StartTime == localStartTime);

        if (requestedSlot is null)
            throw new DomainException("The requested time slot is not available.");

        var startUtc = TimeZoneInfo.ConvertTimeToUtc(
            DateTime.SpecifyKind(requestedSlot.StartTime, DateTimeKind.Unspecified),
            timeZone);

        var endUtc = TimeZoneInfo.ConvertTimeToUtc(
            DateTime.SpecifyKind(requestedSlot.EndTime, DateTimeKind.Unspecified),
            timeZone);

        var startTime = new DateTimeOffset(startUtc);
        var endTime = new DateTimeOffset(endUtc);

        var hasOverlap = await _appointmentRepository
            .HasOverlappingAppointmentAsync(
                request.BusinessMemberId,
                startTime,
                endTime,
                cancellationToken);

        if (hasOverlap)
            throw new DomainException("The requested time slot is already booked.");

        var customer = await ResolveCustomerAsync(
            businessId, request, cancellationToken);

        var appointment = new Appointment(
            businessId,
            customer.Id,
            request.ServiceId,
            request.BusinessMemberId,
            startTime,
            endTime,
            request.Notes);

        await _appointmentRepository.AddAsync(appointment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(appointment);
    }

    public async Task<AppointmentListResponse> GetByBusinessIdAsync(
        Guid businessId,
        AppointmentQueryRequest query,
        CancellationToken cancellationToken)
    {
        if (query.From.HasValue && query.To.HasValue &&
            query.From.Value > query.To.Value)
            throw new ArgumentException("'From' cannot be later than 'To'.");

        var business = await _businessRepository.GetByIdAsync(
            businessId, cancellationToken);

        if (business is null)
            throw new KeyNotFoundException("Business not found.");

        if (_currentUser.UserId is not Guid userId)
            throw new UnauthorizedAccessException("Authentication is required.");

        Guid? businessMemberId = null;
        Guid? customerId = null;

        var businessMember = await _businessMemberRepository
            .GetByBusinessAndUserIdAsync(businessId, userId, cancellationToken);

        if (businessMember is not null)
        {
            switch (businessMember.Role)
            {
                case BusinessRole.Owner:
                case BusinessRole.Admin:
                    break;

                case BusinessRole.Staff:
                    businessMemberId = businessMember.Id;
                    break;

                default:
                    throw new UnauthorizedAccessException("Invalid business role.");
            }
        }
        else
        {
            var customer = await _customerRepository
                .GetByBusinessAndUserIdAsync(businessId, userId, cancellationToken);

            if (customer is null)
                throw new UnauthorizedAccessException(
                    "You do not have access to this business's appointments.");

            customerId = customer.Id;
        }

        var (items, totalCount) = await _appointmentRepository
            .GetByBusinessIdAsync(
                businessId, query, businessMemberId, customerId,
                cancellationToken);

        return new AppointmentListResponse
        {
            Items = items.Select(MapToResponse).ToList(),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / query.PageSize)
        };
    }

    public async Task<AppointmentResponse> GetByIdAsync(
        Guid appointmentId,
        CancellationToken cancellationToken)
    {
        var userId = GetAuthenticatedUserId();

        var appointment = await GetAppointmentOrThrowAsync(
            appointmentId, cancellationToken);

        await EnsureAppointmentAccessAsync(
            appointment, userId, cancellationToken);

        return MapToResponse(appointment);
    }

    public async Task<AppointmentResponse> CancelAsync(
        Guid appointmentId,
        CancellationToken cancellationToken)
    {
        var userId = GetAuthenticatedUserId();

        var appointment = await GetAppointmentOrThrowAsync(
            appointmentId, cancellationToken);

        await EnsureAppointmentAccessAsync(
            appointment, userId, cancellationToken);

        appointment.Cancel();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(appointment);
    }

    public async Task<AppointmentResponse> ConfirmAsync(
        Guid appointmentId,
        CancellationToken cancellationToken)
    {
        var userId = GetAuthenticatedUserId();

        var appointment = await GetAppointmentOrThrowAsync(
            appointmentId, cancellationToken);

        await EnsureBusinessMemberAccessAsync(
            appointment, userId, cancellationToken);

        appointment.Confirm();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(appointment);
    }

    public async Task<AppointmentResponse> CompleteAsync(
        Guid appointmentId,
        CancellationToken cancellationToken)
    {
        var userId = GetAuthenticatedUserId();

        var appointment = await GetAppointmentOrThrowAsync(
            appointmentId, cancellationToken);

        await EnsureBusinessMemberAccessAsync(
            appointment, userId, cancellationToken);

        appointment.Complete();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(appointment);
    }

    private async Task<Customer> ResolveCustomerAsync(
        Guid businessId,
        CreateAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(
                userId, cancellationToken);

            if (user is null)
                throw new UnauthorizedAccessException("User not found.");

            var existingCustomer = await _customerRepository
                .GetByBusinessAndUserIdAsync(
                    businessId, userId, cancellationToken);

            if (existingCustomer is not null)
                return existingCustomer;

            var customer = new Customer(
                businessId,
                user.FirstName,
                user.LastName,
                user.Email,
                request.Phone,
                userId);

            await _customerRepository.AddAsync(customer, cancellationToken);

            return customer;
        }

        if (string.IsNullOrWhiteSpace(request.FirstName))
            throw new ArgumentException("First name is required.");

        if (string.IsNullOrWhiteSpace(request.LastName))
            throw new ArgumentException("Last name is required.");

        var email = string.IsNullOrWhiteSpace(request.Email)
            ? null : request.Email.Trim();

        var phone = string.IsNullOrWhiteSpace(request.Phone)
            ? null : request.Phone.Trim();

        if (email is null && phone is null)
            throw new ArgumentException(
                "At least an email or phone number is required.");

        var guestCustomer = new Customer(
            businessId,
            request.FirstName.Trim(),
            request.LastName.Trim(),
            email,
            phone);

        await _customerRepository.AddAsync(guestCustomer, cancellationToken);

        return guestCustomer;
    }

    private async Task EnsureAppointmentAccessAsync(
        Appointment appointment,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var businessMember = await _businessMemberRepository
            .GetByBusinessAndUserIdAsync(
                appointment.BusinessId, userId, cancellationToken);

        if (businessMember is not null)
        {
            var hasAccess = businessMember.Role switch
            {
                BusinessRole.Owner => true,
                BusinessRole.Admin => true,
                BusinessRole.Staff =>
                    appointment.BusinessMemberId == businessMember.Id,
                _ => false
            };

            if (hasAccess)
                return;
        }
        else
        {
            var customer = await _customerRepository
                .GetByBusinessAndUserIdAsync(
                    appointment.BusinessId, userId, cancellationToken);

            if (customer is not null &&
                appointment.CustomerId == customer.Id)
                return;
        }

        throw new UnauthorizedAccessException(
            "You do not have access to this appointment.");
    }

    private async Task EnsureBusinessMemberAccessAsync(
        Appointment appointment,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var businessMember = await _businessMemberRepository
            .GetByBusinessAndUserIdAsync(
                appointment.BusinessId, userId, cancellationToken);

        if (businessMember is null)
            throw new UnauthorizedAccessException(
                "Only business members can perform this action.");

        var hasAccess = businessMember.Role switch
        {
            BusinessRole.Owner => true,
            BusinessRole.Admin => true,
            BusinessRole.Staff =>
                appointment.BusinessMemberId == businessMember.Id,
            _ => false
        };

        if (!hasAccess)
            throw new UnauthorizedAccessException(
                "You cannot manage this appointment.");
    }

    private Guid GetAuthenticatedUserId()
    {
        if (_currentUser.UserId is not Guid userId)
            throw new UnauthorizedAccessException("Authentication is required.");

        return userId;
    }

    private async Task<Appointment> GetAppointmentOrThrowAsync(
        Guid appointmentId,
        CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(
            appointmentId, cancellationToken);

        return appointment ??
            throw new KeyNotFoundException("Appointment not found.");
    }

    private static AppointmentResponse MapToResponse(Appointment appointment)
    {
        return new AppointmentResponse
        {
            Id = appointment.Id,
            BusinessId = appointment.BusinessId,
            CustomerId = appointment.CustomerId,
            ServiceId = appointment.ServiceId,
            BusinessMemberId = appointment.BusinessMemberId,
            StartTime = appointment.StartTime,
            EndTime = appointment.EndTime,
            Status = appointment.Status.ToString(),
            Notes = appointment.Notes,
            CreatedAt = appointment.CreatedAt
        };
    }
}
