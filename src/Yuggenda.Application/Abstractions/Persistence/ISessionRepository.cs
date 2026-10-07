using Yuggenda.Domain.Entities;

namespace Yuggenda.Application.Abstractions.Persistence;

public interface ISessionRepository
{
    Task AddAsync(
        Session session,
        CancellationToken cancellationToken
    );
    Task RevokeAllByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken
    );
    Task<Session?> GetByRefreshTokenHashAsync(
        string refreshTokenHash,
        CancellationToken cancellationToken
    );
}