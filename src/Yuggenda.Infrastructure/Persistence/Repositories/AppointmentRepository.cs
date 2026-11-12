using Microsoft.EntityFrameworkCore;
using Yuggenda.Application.Abstractions.Persistence;
using Yuggenda.Application.DTOs.Appointments;
using Yuggenda.Domain.Entities;
using Yuggenda.Domain.Enums;
using Yuggenda.Infrastructure.Persistence.Context;

namespace Yuggenda.Infrastructure.Persistence.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly YuggendaDbContext _context;

    public AppointmentRepository(YuggendaDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        Appointment appointment,
        CancellationToken cancellationToken)
    {
        await _context.Appointments.AddAsync(
            appointment,
            cancellationToken);
    }

    public async Task<Appointment?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Appointments
            .FirstOrDefaultAsync(
                appointment => appointment.Id == id,
                cancellationToken);
    }

    public async Task<bool> HasOverlappingAppointmentAsync(
        Guid businessMemberId,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        CancellationToken cancellationToken)
    {
        return await _context.Appointments.AnyAsync(
            appointment =>
                appointment.BusinessMemberId == businessMemberId &&
                (appointment.Status == AppointmentStatus.Scheduled ||
                 appointment.Status == AppointmentStatus.Confirmed) &&
                appointment.StartTime < endTime &&
                appointment.EndTime > startTime,
            cancellationToken);
    }

    public async Task<(List<Appointment> Items, int TotalCount)> GetByBusinessIdAsync(
        Guid businessId,
        AppointmentQueryRequest query,
        Guid? businessMemberId,
        Guid? customerId,
        CancellationToken cancellationToken)
    {
        var appointments = _context.Appointments
            .AsNoTracking()
            .Where(appointment => appointment.BusinessId == businessId);

        if (businessMemberId.HasValue)
        {
            appointments = appointments.Where(
                appointment => appointment.BusinessMemberId == businessMemberId.Value);
        }

        if (customerId.HasValue)
        {
            appointments = appointments.Where(
                appointment => appointment.CustomerId == customerId.Value);
        }

        if (query.From.HasValue)
        {
            appointments = appointments.Where(
                appointment => appointment.StartTime >= query.From.Value);
        }

        if (query.To.HasValue)
        {
            appointments = appointments.Where(
                appointment => appointment.StartTime <= query.To.Value);
        }

        if (query.Status.HasValue)
        {
            appointments = appointments.Where(
                appointment => appointment.Status == query.Status.Value);
        }

        if (query.BusinessMemberId.HasValue)
        {
            appointments = appointments.Where(
                appointment => appointment.BusinessMemberId == query.BusinessMemberId.Value);
        }

        if (query.CustomerId.HasValue)
        {
            appointments = appointments.Where(
                appointment => appointment.CustomerId == query.CustomerId.Value);
        }

        var totalCount = await appointments.CountAsync(cancellationToken);

        var items = await appointments
            .OrderBy(appointment => appointment.StartTime)
            .ThenBy(appointment => appointment.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}