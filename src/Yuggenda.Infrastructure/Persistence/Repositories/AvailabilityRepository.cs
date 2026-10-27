using Microsoft.EntityFrameworkCore;
using Yuggenda.Application.Abstractions.Persistence;
using Yuggenda.Domain.Entities;
using Yuggenda.Domain.Enums;
using Yuggenda.Infrastructure.Persistence.Context;

namespace Yuggenda.Infrastructure.Persistence.Repositories;

public class AvailabilityRepository : IAvailabilityRepository
{
    private readonly YuggendaDbContext _context;

    public AvailabilityRepository(YuggendaDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        Availability availability,
        CancellationToken cancellationToken
    )
    {
        await _context.Availabilities.AddAsync(availability, cancellationToken);
    }

    public async Task<Availability?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return await _context.Availabilities
            .FirstOrDefaultAsync(
                availability => availability.Id == id,
                cancellationToken
            );
    }

    public async Task<List<Availability>> GetByBusinessIdAsync(
        Guid businessId,
        CancellationToken cancellationToken
    )
    {
        return await _context.Availabilities
            .Where(availability =>
                availability.BusinessMember.BusinessId == businessId)
            .OrderBy(availability => availability.BusinessMemberId)
            .ThenBy(availability => availability.Type)
            .ThenBy(availability => availability.DayOfWeek)
            .ThenBy(availability => availability.SpecificDate)
            .ThenBy(availability => availability.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Availability>> GetByBusinessMemberIdAsync(
        Guid businessMemberId,
        CancellationToken cancellationToken
    )
    {
        return await _context.Availabilities
            .Where(
                availability =>
                    availability.BusinessMemberId == businessMemberId
            )
            .OrderBy(availability => availability.Type)
            .ThenBy(availability => availability.DayOfWeek)
            .ThenBy(availability => availability.SpecificDate)
            .ThenBy(availability => availability.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Availability>> GetByBusinessMemberAndTypeAsync(
        Guid businessMemberId,
        AvailabilityType type,
        CancellationToken cancellationToken
    )
    {
        return await _context.Availabilities
            .Where(
                availability => 
                    availability.BusinessMemberId == businessMemberId
                    && availability.Type == type
            )
            .OrderBy(availability => availability.StartTime)
            .ToListAsync(cancellationToken);
    }

    public Task RemoveAsync(
        Availability availability,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        _context.Availabilities.Remove(availability);
        
        return Task.CompletedTask;
    }
}