using Yuggenda.Domain.Entities;

namespace Yuggenda.Application.Abstractions.Persistence;

public interface IBusinessRepository
{
    Task AddAsync(Business business, CancellationToken cancellationToken);
    Task<List<Business>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<Business?> GetByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<List<Business>> GetAllAsync(CancellationToken cancellationToken);
}