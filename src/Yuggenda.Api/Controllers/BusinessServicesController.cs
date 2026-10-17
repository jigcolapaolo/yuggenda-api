using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yuggenda.Application.Services;
using Yuggenda.Application.DTOs.Services;
using Yuggenda.Api.Authorization;

namespace Yuggenda.Api.Controllers;

[ApiController]
[Route("businesses/{businessId:guid}/services")]
public class BusinessServicesController : ControllerBase
{
    private readonly ServiceService _serviceService;

    public BusinessServicesController(ServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    [HttpPost]
    [Authorize]
    [BusinessResourceAuthorize(BusinessPolicies.ManageServices)]
    public async Task<ActionResult<ServiceResponse>> Create(
        Guid businessId,
        CreateServiceRequest request,
        CancellationToken cancellationToken
    )
    {
        var service = await _serviceService.CreateAsync(
            businessId,
            request,
            cancellationToken
        );

        return CreatedAtAction(
            "GetServiceById",
            new { id = service.Id },
            service
        );
    }

    [HttpGet]
    public async Task<ActionResult<List<ServiceResponse>>> GetByBusinessId(
        Guid businessId,
        CancellationToken cancellationToken
    )
    {
        var services = await _serviceService.GetByBusinessIdAsync(
            businessId,
            cancellationToken
        );

        return Ok(services);
    }
}