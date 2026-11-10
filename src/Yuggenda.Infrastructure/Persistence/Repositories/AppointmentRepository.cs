using Microsoft.EntityFrameworkCore;
using Yuggenda.Application.Abstractions.Persistence;
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
}