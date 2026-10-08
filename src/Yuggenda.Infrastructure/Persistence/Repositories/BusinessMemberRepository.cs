using Microsoft.EntityFrameworkCore;
using Yuggenda.Application.Abstractions.Persistence;
using Yuggenda.Domain.Entities;
using Yuggenda.Infrastructure.Persistence.Context;

namespace Yuggenda.Infrastructure.Persistence.Repositories;

public class BusinessMemberRepository : IBusinessMemberRepository
{
    private readonly YuggendaDbContext _context;

    public BusinessMemberRepository(YuggendaDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        BusinessMember businessMember,
        CancellationToken cancellationToken
    )
    {
        await _context.BusinessMembers.AddAsync(businessMember, cancellationToken);
    }

    public async Task<List<BusinessMember>> GetByBusinessIdAsync(
        Guid businessId,
        CancellationToken cancellationToken
    )
    {
        return await _context.BusinessMembers
            .Where(member => member.BusinessId == businessId)
            .ToListAsync(cancellationToken);
    }

    public async Task<BusinessMember?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return await _context.BusinessMembers
            .FirstOrDefaultAsync(
                member => member.Id == id,
                cancellationToken);
    }

    public async Task<BusinessMember?> GetByBusinessAndUserIdAsync(
        Guid businessId,
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        return await _context.BusinessMembers
            .FirstOrDefaultAsync(
                member =>
                    member.BusinessId == businessId &&
                    member.UserId == userId,
                cancellationToken);
    }
}