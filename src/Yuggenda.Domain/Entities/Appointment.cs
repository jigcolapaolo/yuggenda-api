using Yuggenda.Domain.Enums;
using Yuggenda.Domain.Exceptions;

namespace Yuggenda.Domain.Entities;

public class Appointment
{
    public Guid Id { get; private set; }
    public Guid BusinessId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid ServiceId { get; private set; }
    public Guid BusinessMemberId { get; private set; }
    public DateTimeOffset StartTime { get; private set; }
    public DateTimeOffset EndTime { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public Business Business { get; private set; } = null!;
    public Customer Customer { get; private set; } = null!;
    public Service Service { get; private set; } = null!;
    public BusinessMember BusinessMember { get; private set; } = null!;

    public Appointment(
        Guid businessId,
        Guid customerId,
        Guid serviceId,
        Guid businessMemberId,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        string? notes = null
    )
    {
        if (startTime <= DateTimeOffset.UtcNow)
            throw new DomainException(
                "Appointments cannot be created in the past.");

        if (endTime <= startTime)
            throw new DomainException(
                "End time must be after start time.");

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

    public void Confirm()
    {
        if (Status != AppointmentStatus.Scheduled)
            throw new DomainException(
                "Only scheduled appointments can be confirmed.");

        Status = AppointmentStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (Status != AppointmentStatus.Confirmed)
            throw new DomainException(
                "Only confirmed appointments can be completed.");

        if (EndTime > DateTimeOffset.UtcNow)
            throw new DomainException(
                "An appointment cannot be completed before it ends.");

        Status = AppointmentStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status is AppointmentStatus.Completed or AppointmentStatus.Cancelled)
            throw new DomainException(
                "Completed or cancelled appointments cannot be cancelled.");

        Status = AppointmentStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reschedule(
        DateTimeOffset startTime,
        DateTimeOffset endTime)
    {
        if (Status is AppointmentStatus.Completed or AppointmentStatus.Cancelled)
            throw new DomainException(
                "Completed or cancelled appointments cannot be rescheduled.");

        if (startTime <= DateTimeOffset.UtcNow)
            throw new DomainException(
                "Appointments cannot be rescheduled to the past.");

        if (endTime <= startTime)
            throw new DomainException(
                "End time must be after start time.");

        StartTime = startTime;
        EndTime = endTime;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeService(Guid serviceId, DateTimeOffset endTime)
    {
        if (Status is AppointmentStatus.Completed or AppointmentStatus.Cancelled)
            throw new DomainException(
                "Completed or cancelled appointments cannot change service.");

        if (serviceId == Guid.Empty)
            throw new DomainException("Service ID is required.");

        if (endTime <= StartTime)
            throw new DomainException("End time must be after start time.");

        ServiceId = serviceId;
        EndTime = endTime;
        UpdatedAt = DateTime.UtcNow;
    }
}
