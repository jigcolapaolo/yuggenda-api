using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yuggenda.Api.Authorization;
using Yuggenda.Application.DTOs.Availabilities;
using Yuggenda.Application.Services.Availabilities;

namespace Yuggenda.Api.Controllers;

[ApiController]
[Route("availabilities")]
public class AvailabilitiesController : ControllerBase
{
    private readonly AvailabilityService _availabilityService;

    public AvailabilitiesController(
        AvailabilityService availabilityService
    )
    {
        _availabilityService = availabilityService;
    }

    [Authorize]
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<AvailabilityResponse>> Update(
        Guid id,
        [FromBody] UpdateAvailabilityRequest request,
        CancellationToken cancellationToken
    )
    {
        var availability = await _availabilityService.UpdateAsync(
            id,
            request,
            cancellationToken
        );

        return Ok(availability);
    }
}