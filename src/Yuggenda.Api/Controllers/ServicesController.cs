using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yuggenda.Application.Services;
using Yuggenda.Application.DTOs.Services;
using Yuggenda.Api.Authorization;

namespace Yuggenda.Api.Controllers;

[ApiController]
[Route("services")]
public class ServicesController : ControllerBase
{
    private readonly ServiceService _serviceService;
    private readonly IAuthorizationService _authorizationService;

    public ServicesController(
        ServiceService serviceService,
        IAuthorizationService authorizationService
    )
    {
        _serviceService = serviceService;
        _authorizationService = authorizationService;
    }

    [HttpGet("{id:guid}", Name = "GetServiceById")]
    public async Task<ActionResult<ServiceResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        var service = await _serviceService.GetByIdAsync(id, cancellationToken);

        return Ok(service);
    }

    [Authorize]
    [ServiceResourceAuthorize(BusinessPolicies.ManageServices)]
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<ServiceResponse>> Update(
        Guid id,
        UpdateServiceRequest request,
        CancellationToken cancellationToken
    )
    {
        var service = await _serviceService.UpdateAsync(
            id,
            request,
            cancellationToken
        );

        return Ok(service);
    }

    
    [Authorize]
    [ServiceResourceAuthorize(BusinessPolicies.ManageServices)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deactivate(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        await _serviceService.DeactivateAsync(id, cancellationToken);

        return NoContent();
    }

    [Authorize]
    [ServiceResourceAuthorize(BusinessPolicies.ManageServices)]
    [HttpPatch("{id:guid}/activate")]
    public async Task<IActionResult> Activate(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        await _serviceService.ActivateAsync(id, cancellationToken);

        return NoContent();
    }

}