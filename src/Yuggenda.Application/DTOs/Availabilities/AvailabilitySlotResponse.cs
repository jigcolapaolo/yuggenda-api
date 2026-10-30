
namespace Yuggenda.Application.DTOs.Availabilities;

public class AvailabilitySlotResponse
{
    public Guid BusinessMemberId { get; init; }

    public Guid ServiceId { get; init; }

    public DateTime StartTime { get; init; }

    public DateTime EndTime { get; init; }
}
