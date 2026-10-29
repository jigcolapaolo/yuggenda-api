
using Microsoft.AspNetCore.Mvc;
using Yuggenda.Application.DTOs.Availabilities;
using Yuggenda.Application.Services.Availabilities;

namespace Yuggenda.Api.Controllers;

[ApiController]
[Route("businesses/{businessId:guid}/availability/slots")]
public class BusinessAvailabilitySlotsController : ControllerBase
{
    private readonly AvailabilityCalculationService _availabilityCalculationService;

    public BusinessAvailabilitySlotsController(
        AvailabilityCalculationService availabilityCalculationService
    )
    {
        _availabilityCalculationService = availabilityCalculationService;
    }

    [HttpGet]
    public async Task<ActionResult<List<AvailabilitySlotResponse>>> GetAvailableSlots(
        Guid businessId,
        [FromQuery] Guid serviceId,
        [FromQuery] DateOnly date,
        [FromQuery] Guid? businessMemberId,
        CancellationToken cancellationToken
    )
    {
        var slots = await _availabilityCalculationService.GetAvailableSlotsForServiceAsync(
            businessId,
            serviceId,
            date,
            cancellationToken,
            businessMemberId
        );

        return Ok(slots);
    }
}
