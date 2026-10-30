using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yuggenda.Api.Authorization;
using Yuggenda.Application.DTOs.Availabilities;
using Yuggenda.Application.Services.Availabilities;

namespace Yuggenda.Api.Controllers;

[ApiController]
[Route("businesses/{businessId:guid}/availabilities")]
public class BusinessAvailabilitiesController : ControllerBase
{
    private readonly AvailabilityService _availabilityService;

    public BusinessAvailabilitiesController(
        AvailabilityService availabilityService
    )
    {
        _availabilityService = availabilityService;
    }

    [Authorize]
    [BusinessResourceAuthorize(BusinessPolicies.ManageAvailability)]
    [HttpPost]
    public async Task<ActionResult<AvailabilityResponse>> Create(
        Guid businessId,
        [FromQuery] Guid businessMemberId,
        [FromBody] CreateAvailabilityRequest request,
        CancellationToken cancellationToken
    )
    {
        var availability = await _availabilityService.CreateAsync(
            businessId,
            businessMemberId,
            request,
            cancellationToken
        );

        return Created(
            $"/availabilities/{availability.Id}",
            availability
        );
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<List<AvailabilityResponse>>> GetByBusinessId(
        Guid businessId,
        CancellationToken cancellationToken
    )
    {
        var availabilities = await _availabilityService.GetByBusinessIdAsync(
            businessId,
            cancellationToken
        );

        return Ok(availabilities);
    }
}