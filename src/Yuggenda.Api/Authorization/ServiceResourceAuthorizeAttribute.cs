
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Yuggenda.Application.Abstractions.Persistence;

namespace Yuggenda.Api.Authorization;

public sealed class ServiceResourceAuthorizeAttribute
    : TypeFilterAttribute
{
    public ServiceResourceAuthorizeAttribute(string policyName)
        : base(typeof(ServiceResourceAuthorizationFilter))
    {
        Arguments = new object[] { policyName };
    }
}

public sealed class ServiceResourceAuthorizationFilter(
    IServiceRepository serviceRepository,
    IAuthorizationService authorizationService,
    string policyName)
    : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var idValue = context.RouteData.Values["id"]?.ToString();

        if (!Guid.TryParse(idValue, out var serviceId))
        {
            context.Result = new NotFoundResult();
            return;
        }

        var service = await serviceRepository.GetByIdAsync(
            serviceId,
            context.HttpContext.RequestAborted);

        if (service is null)
        {
            context.Result = new NotFoundResult();
            return;
        }

        var result = await authorizationService.AuthorizeAsync(
            context.HttpContext.User,
            service.BusinessId,
            policyName);

        if (!result.Succeeded)
        {
            context.Result = new ForbidResult();
            return;
        }

        await next();
    }
}
