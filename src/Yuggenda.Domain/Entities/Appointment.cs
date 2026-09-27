using Yuggenda.Domain.Enums;

namespace Yuggenda.Domain.Entities;

public class Appointment
{
    public Guid Id { get; private set; }
    public Guid BusinessId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid ServiceId { get; private set; }
    public Guid BusinessMemberId { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public Appointment(
        Guid businessId,
        Guid customerId,
        Guid serviceId,
        Guid businessMemberId,
        DateTime startTime,
        DateTime endTime,
        string? notes = null
    )
    {
        Id = Guid.NewGuid();
        BusinessId = businessId;
        CustomerId = customerId;
        ServiceId = serviceId;
        BusinessMemberId = businessMemberId;
        StartTime = startTime;
        EndTime = endTime;
        Status = AppointmentStatus.Scheduled;
        Notes = notes;
        CreatedAt = DateTime.UtcNow;
    }
}