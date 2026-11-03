using Yuggenda.Application.Abstractions.Authentication;
using Yuggenda.Application.Abstractions.Persistence;
using Yuggenda.Application.DTOs.Customers;
using Yuggenda.Domain.Entities;
using Yuggenda.Domain.Exceptions;

namespace Yuggenda.Application.Services.Customers;

public class CustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IBusinessRepository _businessRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public CustomerService(
        ICustomerRepository customerRepository,
        IBusinessRepository businessRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser)
    {
        _customerRepository = customerRepository;
        _businessRepository = businessRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<CustomerResponse> CreateAsync(
        Guid businessId,
        CreateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var business = await _businessRepository.GetByIdAsync(
            businessId,
            cancellationToken);

        if (business is null)
            throw new KeyNotFoundException("Business not found.");

        // Si se proporciona UserId, se verifica que el usuario exista
        if (request.UserId.HasValue)
        {
            if (request.UserId.HasValue && request.UserId.Value != _currentUser.UserId)
            {
                throw new UnauthorizedAccessException(
                    "You can only link a customer to your own user account.");
            }

            var user = await _userRepository.GetByIdAsync(
                request.UserId.Value,
                cancellationToken);

            if (user is null)
                throw new KeyNotFoundException("User not found.");

            var alreadyExists = await _customerRepository.ExistsForUserAsync(
                businessId,
                request.UserId.Value,
                cancellationToken);

            if (alreadyExists)
                throw new DomainException(
                    "This user already has a customer record in this business.");
        }

        var customer = new Customer(
            businessId,
            request.FirstName.Trim(),
            request.LastName.Trim(),
            request.Email?.Trim(),
            request.Phone?.Trim(),
            request.UserId);

        await _customerRepository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(customer);
    }

    public async Task<List<CustomerResponse>> GetByBusinessIdAsync(
        Guid businessId,
        CancellationToken cancellationToken)
    {
        var business = await _businessRepository.GetByIdAsync(
            businessId,
            cancellationToken);

        if (business is null)
            throw new KeyNotFoundException("Business not found.");

        var customers = await _customerRepository.GetByBusinessIdAsync(
            businessId,
            cancellationToken);

        return customers.Select(MapToResponse).ToList();
    }

    public async Task<CustomerResponse> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (customer is null)
            throw new KeyNotFoundException("Customer not found.");

        return MapToResponse(customer);
    }

    public async Task<CustomerResponse> UpdateAsync(
        Guid id,
        UpdateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (customer is null)
            throw new KeyNotFoundException("Customer not found.");

        if (!customer.UserId.HasValue ||
            customer.UserId.Value != _currentUser.UserId)
        {
            throw new UnauthorizedAccessException(
                "You can only update your own customer information.");
        }

        var firstName = request.FirstNameProvided
            ? request.FirstName?.Trim()
            : customer.FirstName;

        var lastName = request.LastNameProvided
            ? request.LastName?.Trim()
            : customer.LastName;

        var email = request.EmailProvided
            ? request.Email?.Trim()
            : customer.Email;

        var phone = request.PhoneProvided
            ? request.Phone?.Trim()
            : customer.Phone;

        email = string.IsNullOrWhiteSpace(email) ? null : email;
        phone = string.IsNullOrWhiteSpace(phone) ? null : phone;

        if (string.IsNullOrWhiteSpace(firstName) ||
            string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException(
                "First name and last name cannot be empty.");
        }

        if (email is null && phone is null)
        {
            throw new ArgumentException(
                "At least one contact method (email or phone) is required.");
        }

        customer.UpdateContactInfo(
            firstName,
            lastName,
            email,
            phone);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(customer);
    }

    private static CustomerResponse MapToResponse(Customer customer)
    {
        return new CustomerResponse
        {
            Id = customer.Id,
            BusinessId = customer.BusinessId,
            UserId = customer.UserId,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            Phone = customer.Phone,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }
}