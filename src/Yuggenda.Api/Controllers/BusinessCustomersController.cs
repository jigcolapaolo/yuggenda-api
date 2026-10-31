using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yuggenda.Api.Authorization;
using Yuggenda.Application.DTOs.Customers;
using Yuggenda.Application.Services.Customers;

namespace Yuggenda.Api.Controllers;

[ApiController]
[Route("businesses/{businessId:guid}/customers")]
[Authorize]
public class BusinessCustomersController : ControllerBase
{
    private readonly CustomerService _customerService;

    public BusinessCustomersController(CustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpPost]
    [BusinessResourceAuthorize(BusinessPolicies.ManageCustomers)]
    public async Task<ActionResult<CustomerResponse>> Create(
        Guid businessId,
        CreateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerService.CreateAsync(
            businessId,
            request,
            cancellationToken);

        return StatusCode(StatusCodes.Status201Created, customer);
    }
}