using Microsoft.EntityFrameworkCore;
using Npgsql;
using Yuggenda.Application.Abstractions.Persistence;
using Yuggenda.Application.Exceptions;
using Yuggenda.Infrastructure.Persistence.Context;

namespace Yuggenda.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly YuggendaDbContext _context;

    public UnitOfWork(YuggendaDbContext context)
    {
        _context = context;
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is PostgresException postgresException
                && postgresException.SqlState ==
                    PostgresErrorCodes.RestrictViolation)
        {
            throw new ConflictException(
                "The operation cannot be completed because related records exist."
            );
        }
    }
}