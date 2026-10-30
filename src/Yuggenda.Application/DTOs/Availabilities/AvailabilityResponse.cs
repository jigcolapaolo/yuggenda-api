using Yuggenda.Domain.Enums;

namespace Yuggenda.Application.DTOs.Availabilities;

public class AvailabilityResponse
{
    public Guid Id { get; init; }

    public Guid BusinessMemberId { get; init; }

    public AvailabilityType Type { get; init; }

    public DayOfWeek? DayOfWeek { get; init; }

    public DateOnly? SpecificDate { get; init; }

    public TimeOnly StartTime { get; init; }

    public TimeOnly EndTime { get; init; }

    public bool IsAvailable { get; init; }
}
