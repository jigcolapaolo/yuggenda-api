using Microsoft.EntityFrameworkCore;
using Yuggenda.Application.Abstractions.Persistence;
using Yuggenda.Domain.Entities;
using Yuggenda.Infrastructure.Persistence.Context;

namespace Yuggenda.Infrastructure.Persistence.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly YuggendaDbContext _context;

    public SessionRepository(YuggendaDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        Session session,
        CancellationToken cancellationToken)
    {
        await _context.Sessions.AddAsync(
            session,
            cancellationToken);
    }

    public async Task RevokeAllByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        var sessions = await _context.Sessions
            .Where(session => session.UserId == userId)
            .ToListAsync(cancellationToken);

        foreach (var session in sessions)
        {
            session.Revoke();
        }
    }
}