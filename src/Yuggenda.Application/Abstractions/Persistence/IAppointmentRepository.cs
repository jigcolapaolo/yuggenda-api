using Yuggenda.Domain.Entities;

namespace Yuggenda.Application.Abstractions.Persistence;

public interface IAppointmentRepository
{
    Task AddAsync(
        Appointment appointment,
        CancellationToken cancellationToken
    );

    Task<Appointment?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken
    );

    Task<bool> HasOverlappingAppointmentAsync(
        Guid businessMemberId,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        CancellationToken cancellationToken
    );
}