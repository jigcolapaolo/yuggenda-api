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