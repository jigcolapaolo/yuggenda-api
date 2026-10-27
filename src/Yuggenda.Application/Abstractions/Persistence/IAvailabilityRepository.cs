using Yuggenda.Domain.Entities;
using Yuggenda.Domain.Enums;

namespace Yuggenda.Application.Abstractions.Persistence;

public interface IAvailabilityRepository
{
    Task AddAsync(
        Availability availability,
        CancellationToken cancellationToken
    );

    Task<Availability?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken
    );

    Task<List<Availability>> GetByBusinessIdAsync(
        Guid businessId,
        CancellationToken cancellationToken
    );

    Task<List<Availability>> GetByBusinessMemberIdAsync(
        Guid businessMemberId,
        CancellationToken cancellationToken
    );

    Task<List<Availability>> GetByBusinessMemberAndTypeAsync(
        Guid businessMemberId,
        AvailabilityType type,
        CancellationToken cancellationToken
    );

    Task RemoveAsync(
        Availability availability,
        CancellationToken cancellationToken
    );
}