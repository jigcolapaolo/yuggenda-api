using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yuggenda.Application.DTOs.Businesses;
using Yuggenda.Application.Services.Businesses;

namespace Yuggenda.Api.Controllers;

[ApiController]
[Route("businesses")]
public class BusinessesController : ControllerBase
{
    private readonly BusinessService _businessService;

    public BusinessesController(BusinessService businessService)
    {
        _businessService = businessService;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<BusinessResponse>> Create(
        CreateBusinessRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await _businessService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = response.Id },
            response
        );
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BusinessResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        var response = await _businessService.GetByIdAsync(id, cancellationToken);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<List<BusinessResponse>>> GetAll(
        CancellationToken cancellationToken
    )
    {
        var response = await _businessService.GetAllAsync(cancellationToken);

        return Ok(response);
    }

    [HttpGet("mine")]
    [Authorize]
    public async Task<ActionResult<List<BusinessResponse>>> GetMine(
        CancellationToken cancellationToken
    )
    {
        var response = await _businessService.GetMineAsync(cancellationToken);

        return Ok(response);
    }

    [HttpPatch("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<BusinessResponse>> Update(
        Guid id,
        UpdateBusinessRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await _businessService.UpdateAsync(id, request, cancellationToken);

        return Ok(response);
    }
}