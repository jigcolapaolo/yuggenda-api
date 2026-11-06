using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yuggenda.Api.Authorization;
using Yuggenda.Application.DTOs.Customers;
using Yuggenda.Application.Services.Customers;

namespace Yuggenda.Api.Controllers;

[ApiController]
[Route("customers")]
public class CustomersController : ControllerBase
{
    private readonly CustomerService _customerService;
    private readonly IAuthorizationService _authorizationService;

    public CustomersController(
        CustomerService customerService,
        IAuthorizationService authorizationService)
    {
        _customerService = customerService;
        _authorizationService = authorizationService;
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<CustomerResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var customer = await _customerService.GetByIdAsync(
            id,
            cancellationToken);

        var authorizationResult = await _authorizationService.AuthorizeAsync(
            User,
            customer.BusinessId,
            BusinessPolicies.ManageCustomers);

        if (!authorizationResult.Succeeded)
            return Forbid();

        return Ok(customer);
    }

    [HttpPatch("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<CustomerResponse>> Update(
        Guid id,
        UpdateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerService.UpdateAsync(
            id,
            request,
            cancellationToken);

        return Ok(customer);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var customer = await _customerService.GetByIdAsync(
            id,
            cancellationToken);

        var authorizationResult = await _authorizationService.AuthorizeAsync(
            User,
            customer.BusinessId,
            BusinessPolicies.ManageCustomers);

        if (!authorizationResult.Succeeded)
            return Forbid();

        await _customerService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}