using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Yuggenda.Api.Authorization;

public sealed class BusinessResourceAuthorizeAttribute
    : TypeFilterAttribute
{
    public BusinessResourceAuthorizeAttribute(string policyName)
        : base(typeof(BusinessResourceAuthorizationFilter))
    {
        Arguments = new object[] { policyName };
    }
}

public sealed class BusinessResourceAuthorizationFilter(
    IAuthorizationService authorizationService,
    string policyName)
    : IAsyncActionFilter
{

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var routeValues = context.HttpContext.Request.RouteValues;

        var businessIdValue =
            routeValues["businessId"]?.ToString()
            ?? routeValues["id"]?.ToString();

        if (!Guid.TryParse(businessIdValue, out var businessId))
        {
            context.Result = new NotFoundResult();
            return;
        }

        var result = await authorizationService.AuthorizeAsync(
            context.HttpContext.User,
            businessId,
            policyName);

        if (!result.Succeeded)
        {
            context.Result = new ForbidResult();
            return;
        }

        await next();
    }

}
