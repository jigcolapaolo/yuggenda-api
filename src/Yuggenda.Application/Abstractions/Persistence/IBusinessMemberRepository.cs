using Yuggenda.Domain.Entities;

namespace Yuggenda.Application.Abstractions.Persistence;

public interface IBusinessMemberRepository
{
    Task AddAsync(BusinessMember businessMember, CancellationToken cancellationToken);

    Task<List<BusinessMember>> GetByBusinessIdAsync(Guid businessId, CancellationToken cancellationToken);

    Task<BusinessMember?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<BusinessMember?> GetByBusinessAndUserIdAsync(
        Guid businessId, 
        Guid userId, 
        CancellationToken cancellationToken
    );
}