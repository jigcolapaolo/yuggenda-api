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
        if (businessMemberId == Guid.Empty)
        {
            throw new ArgumentException(
                "Business member ID is required.",
                nameof(businessMemberId)
            );
        }

        if (startTime >= endTime)
        {
            throw new ArgumentException(
                "Start time must be earlier than end time.",
                nameof(startTime)
            );
        }

        switch (type)
        {
            case AvailabilityType.Weekly:
                if (dayOfWeek is null || specificDate is not null)
                {
                    throw new ArgumentException(
                        "Weekly availability requires a day of week and must not specify a date."
                    );
                }

                break;

            case AvailabilityType.Exception:
                if (specificDate is null || dayOfWeek is not null)
                {
                    throw new ArgumentException(
                        "Exception availability requires a specific date and must not specify a day of week."
                    );
                }

                break;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(type),
                    "Availability type is invalid."
                );
        }

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

    public void Update(
        TimeOnly startTime,
        TimeOnly endTime,
        bool isAvailable,
        DayOfWeek? dayOfWeek = null,
        DateOnly? specificDate = null
    )
    {
        if (startTime >= endTime)
        {
            throw new ArgumentException(
                "Start time must be earlier than end time.",
                nameof(startTime)
            );
        }

        switch (Type)
        {
            case AvailabilityType.Weekly:
                if (dayOfWeek is null || specificDate is not null)
                {
                    throw new ArgumentException(
                        "Weekly availability requires a day of week and must not specify a date."
                    );
                }

                break;

            case AvailabilityType.Exception:
                if (specificDate is null || dayOfWeek is not null)
                {
                    throw new ArgumentException(
                        "Exception availability requires a specific date and must not specify a day of week."
                    );
                }

                break;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(Type),
                    "Availability type is invalid."
                );
        }

        StartTime = startTime;
        EndTime = endTime;
        IsAvailable = isAvailable;
        DayOfWeek = dayOfWeek;
        SpecificDate = specificDate;
        UpdatedAt = DateTime.UtcNow;
    }

}
