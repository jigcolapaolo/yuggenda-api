using Microsoft.EntityFrameworkCore;
using Yuggenda.Application.Abstractions.Persistence;
using Yuggenda.Domain.Entities;
using Yuggenda.Infrastructure.Persistence;
using Yuggenda.Infrastructure.Persistence.Context;

namespace Yuggenda.Infrastructure.Persistence.Repositories;

public class ServiceRepository : IServiceRepository
{
    private readonly YuggendaDbContext _context;

    public ServiceRepository(YuggendaDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        Service service,
        CancellationToken cancellationToken
    )
    {
        await _context.Services.AddAsync(service, cancellationToken);
    }

    public async Task<Service?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return await _context.Services
            .Include(service => service.Business)
            .FirstOrDefaultAsync(
                service => service.Id == id,
                cancellationToken
            );
    }


    public async Task<List<Service>> GetByBusinessIdAsync(
        Guid businessId,
        CancellationToken cancellationToken,
        bool activeOnly = false
    )
    {
        var query = _context.Services
            .Where(service => service.BusinessId == businessId);

        if (activeOnly)
        {
            query = query.Where(service => service.IsActive);
        }

        return await query
            .OrderBy(service => service.Name)
            .ToListAsync(cancellationToken);
    }


    public Task RemoveAsync(
        Service service,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        _context.Services.Remove(service);

        return Task.CompletedTask;
    }
}