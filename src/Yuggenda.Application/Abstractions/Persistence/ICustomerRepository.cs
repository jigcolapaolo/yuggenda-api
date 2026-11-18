using Yuggenda.Domain.Entities;

namespace Yuggenda.Application.Abstractions.Persistence;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer, CancellationToken cancellationToken);

    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Customer?> GetByIdAndBusinessIdAsync(
        Guid id,
        Guid businessId,
        CancellationToken cancellationToken);

    Task<List<Customer>> GetByBusinessIdAsync(
        Guid businessId,
        CancellationToken cancellationToken);

    Task<bool> ExistsForUserAsync(
        Guid businessId,
        Guid userId,
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken);


    Task<bool> HasAppointmentsAsync(
        Guid customerId,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Customer customer,
        CancellationToken cancellationToken);

    Task<Customer?> GetByBusinessAndUserIdAsync(
        Guid businessId,
        Guid userId,
        CancellationToken cancellationToken);
}