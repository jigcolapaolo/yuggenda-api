using Yuggenda.Domain.Enums;

namespace Yuggenda.Domain.Entities;

/*
Weekly
→ necesita DayOfWeek
→ no necesita SpecificDate

Exception
→ necesita SpecificDate
→ no necesita DayOfWeek
*/

public class Availability
{
    public Guid Id { get; private set; }
    public Guid BusinessMemberId { get; private set; }
    public AvailabilityType Type { get; private set; }
    public DayOfWeek? DayOfWeek { get; private set; }
    public DateOnly? SpecificDate { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public bool IsAvailable { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public BusinessMember BusinessMember { get; private set; } = null!;

    public Availability(
        Guid businessMemberId,
        AvailabilityType type,
        TimeOnly startTime,
        TimeOnly endTime,
        bool isAvailable,
        DayOfWeek? dayOfWeek = null,
        DateOnly? specificDate = null
    )
    {
        Id = Guid.NewGuid();
        BusinessMemberId = businessMemberId;
        Type = type;
        DayOfWeek = dayOfWeek;
        SpecificDate = specificDate;
        StartTime = startTime;
        EndTime = endTime;
        IsAvailable = isAvailable;
        CreatedAt = DateTime.UtcNow;
    }
}
