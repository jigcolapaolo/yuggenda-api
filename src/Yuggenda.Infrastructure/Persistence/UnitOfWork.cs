using Yuggenda.Application.Abstractions.Persistence;
using Yuggenda.Infrastructure.Persistence.Context;

namespace Yuggenda.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly YuggendaDbContext _context;

    public UnitOfWork(YuggendaDbContext context)
    {
        _context = context;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}