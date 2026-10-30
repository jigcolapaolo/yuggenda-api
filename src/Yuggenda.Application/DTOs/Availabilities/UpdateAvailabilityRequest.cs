using System.ComponentModel.DataAnnotations;

namespace Yuggenda.Application.DTOs.Availabilities;

public class UpdateAvailabilityRequest
{
    public DayOfWeek? DayOfWeek { get; init; }

    public DateOnly? SpecificDate { get; init; }

    public TimeOnly? StartTime { get; init; }

    public TimeOnly? EndTime { get; init; }

    public bool? IsAvailable { get; init; }
}