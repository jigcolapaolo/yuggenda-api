using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yuggenda.Application.Services;
using Yuggenda.Application.DTOs.Services;
using Yuggenda.Api.Authorization;

namespace Yuggenda.Api.Controllers;

[ApiController]
[Route("services")]
public class BusinessServicesController : ControllerBase
{
    private readonly ServiceService _serviceService;

    public BusinessServicesController(ServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    [HttpGet("{id:guid}", Name = "GetServiceById")]
    public async Task<ActionResult<ServiceResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }
}