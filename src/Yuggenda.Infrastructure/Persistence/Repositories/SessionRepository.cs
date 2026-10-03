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
}