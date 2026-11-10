using Yuggenda.Application.Abstractions.Authentication;
using Yuggenda.Application.Abstractions.Persistence;
using Yuggenda.Application.DTOs.Appointments;
using Yuggenda.Application.Services.Availabilities;
using Yuggenda.Domain.Entities;
using Yuggenda.Domain.Exceptions;

namespace Yuggenda.Application.Services.Appointments;

public class AppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUserRepository _userRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IBusinessRepository _businessRepository;
    private readonly AvailabilityCalculationService _availabilityCalculationService;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        ICustomerRepository customerRepository,
        IUserRepository userRepository,
        IServiceRepository serviceRepository,
        IBusinessRepository businessRepository,
        AvailabilityCalculationService availabilityCalculationService,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _customerRepository = customerRepository;
        _userRepository = userRepository;
        _serviceRepository = serviceRepository;
        _businessRepository = businessRepository;
        _availabilityCalculationService = availabilityCalculationService;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    private async Task<Customer> ResolveCustomerAsync(
        Guid businessId,
        CreateAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(
                userId,
                cancellationToken
            );

            if (user is null)
                throw new UnauthorizedAccessException("User not found.");

            var existingCustomer = await _customerRepository
                .GetByBusinessAndUserIdAsync(
                    businessId,
                    userId,
                    cancellationToken
                );

            if (existingCustomer is not null)
                return existingCustomer;

            var customer = new Customer(
                businessId,
                user.FirstName,
                user.LastName,
                user.Email,
                request.Phone,
                userId
            );

            await _customerRepository.AddAsync(
                customer,
                cancellationToken
            );

            return customer;
        }

        if (string.IsNullOrWhiteSpace(request.FirstName))
            throw new ArgumentException("First name is required.");

        if (string.IsNullOrWhiteSpace(request.LastName))
            throw new ArgumentException("Last name is required.");

        var email = string.IsNullOrWhiteSpace(request.Email)
            ? null
            : request.Email.Trim();

        var phone = string.IsNullOrWhiteSpace(request.Phone)
            ? null
            : request.Phone.Trim();

        if (email is null && phone is null)
            throw new ArgumentException(
                "At least an email or phone number is required."
            );

        var guestCustomer = new Customer(
            businessId,
            request.FirstName.Trim(),
            request.LastName.Trim(),
            email,
            phone
        );

        await _customerRepository.AddAsync(
            guestCustomer,
            cancellationToken
        );

        return guestCustomer;
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