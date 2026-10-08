using Microsoft.EntityFrameworkCore;
using Yuggenda.Application.Abstractions.Persistence;
using Yuggenda.Domain.Entities;
using Yuggenda.Infrastructure.Persistence.Context;

namespace Yuggenda.Infrastructure.Persistence.Repositories;

public class BusinessRepository : IBusinessRepository
{
    private readonly YuggendaDbContext _context;

    public BusinessRepository(YuggendaDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        Business business,
        CancellationToken cancellationToken)
    {
        await _context.Businesses.AddAsync(business, cancellationToken);
    }

    public async Task<List<Business>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        return await _context.Businesses
            .Where(business => business.Members
                .Any(member => member.UserId == userId))
            .ToListAsync(cancellationToken);
    }

    public async Task<Business?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return await _context.Businesses
            .FirstOrDefaultAsync(
                business => business.Id == id,
                cancellationToken
            );
    }

    public async Task<List<Business>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Businesses
            .ToListAsync(cancellationToken);
    }
}