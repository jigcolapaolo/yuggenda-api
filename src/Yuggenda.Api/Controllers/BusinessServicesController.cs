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
    private readonly IAuthorizationService _authorizationService;

    public BusinessServicesController(
        ServiceService serviceService,
        IAuthorizationService authorizationService
    )
    {
        _serviceService = serviceService;
        _authorizationService = authorizationService;
    }

    [Authorize]
    [BusinessResourceAuthorize(BusinessPolicies.ManageServices)]
    [HttpPost]
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


    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<List<ServiceResponse>>> GetByBusinessId(
        Guid businessId,
        CancellationToken cancellationToken
    )
    {
        var authorizationResult = await _authorizationService.AuthorizeAsync(
            User,
            businessId,
            BusinessPolicies.ReadServices
        );

        var activeOnly = !authorizationResult.Succeeded;

        var services = await _serviceService.GetByBusinessIdAsync(
            businessId,
            cancellationToken,
            activeOnly
        );

        return Ok(services);
    }

}