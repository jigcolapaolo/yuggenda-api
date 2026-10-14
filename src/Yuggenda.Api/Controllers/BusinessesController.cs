using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yuggenda.Api.Authorization;
using Yuggenda.Application.DTOs.Businesses;
using Yuggenda.Application.DTOs.BusinessMembers;
using Yuggenda.Application.Services.Businesses;
using Yuggenda.Application.Services.BusinessMembers;
using Yuggenda.Domain.Entities;

namespace Yuggenda.Api.Controllers;

[ApiController]
[Route("businesses")]
public class BusinessesController : ControllerBase
{
    private readonly BusinessService _businessService;
    private readonly BusinessMemberService _businessMemberService;

    public BusinessesController(
        BusinessService businessService,
        BusinessMemberService businessMemberService
    )
    {
        _businessService = businessService;
        _businessMemberService = businessMemberService;
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
    [Authorize(Policy = BusinessPolicies.Update)]
    public async Task<ActionResult<BusinessResponse>> Update(
        Guid id,
        UpdateBusinessRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await _businessService.UpdateAsync(id, request, cancellationToken);

        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = BusinessPolicies.Delete)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _businessService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }

    [HttpGet("{businessId:guid}/members")]
    [Authorize(Policy = BusinessPolicies.ReadMembers)]
    public async Task<ActionResult<List<BusinessMemberResponse>>> GetMembers(
        Guid businessId,
        CancellationToken cancellationToken
    )
    {
        var response = await _businessMemberService.GetByBusinessIdAsync(
            businessId,
            cancellationToken
        );

        return Ok(response);
    }

    [HttpPost("{businessId:guid}/members")]
    [Authorize(Policy = BusinessPolicies.AddMembers)]
    public async Task<ActionResult<BusinessMemberResponse>> AddMember(
        Guid businessId,
        AddBusinessMemberRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await _businessMemberService.AddAsync(
            businessId,
            request,
            cancellationToken
        );

        return CreatedAtAction(
            nameof(GetMembers),
            new { businessId },
            response
        );
    }

    [HttpPatch("{businessId:guid}/members/{memberId:guid}")]
    [Authorize(Policy = BusinessPolicies.UpdateMemberRole)]
    public async Task<ActionResult<BusinessMemberResponse>> UpdateMemberRole(
        Guid businessId,
        Guid memberId,
        [FromBody] UpdateBusinessMemberRoleRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await _businessMemberService.UpdateRoleAsync(
            businessId,
            memberId,
            request,
            cancellationToken
        );

        return Ok(response);
    }

    [HttpDelete("{businessId:guid}/members/{memberId:guid}")]
    [Authorize(Policy = BusinessPolicies.RemoveMembers)]
    public async Task<IActionResult> RemoveMember(
        Guid businessId,
        Guid memberId,
        CancellationToken cancellationToken
    )
    {
        await _businessMemberService.RemoveAsync(
            businessId,
            memberId,
            cancellationToken
        );

        return NoContent();
    }

    [HttpDelete("{businessId:guid}/members/me")]
    [Authorize]
    public async Task<IActionResult> LeaveBusiness(
        Guid businessId,
        CancellationToken cancellationToken
    )
    {
        await _businessMemberService.LeaveAsync(businessId, cancellationToken);

        return NoContent();
    }
}