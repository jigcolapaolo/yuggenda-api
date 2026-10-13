
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Yuggenda.Application.Abstractions.Authentication;
using Yuggenda.Application.Abstractions.Persistence;

namespace Yuggenda.Api.Authorization;

public sealed class BusinessPermissionHandler(
    IBusinessMemberRepository businessMemberRepository,
    ICurrentUser currentUser)
    : AuthorizationHandler<BusinessPermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        BusinessPermissionRequirement requirement)
    {
        var httpContext = context.Resource switch
        {
            HttpContext http => http,
            AuthorizationFilterContext mvcContext => mvcContext.HttpContext,
            _ => null
        };

        if (httpContext is null)
            return;


        var businessIdValue =
            httpContext.Request.RouteValues["businessId"]?.ToString()
            ?? httpContext.Request.RouteValues["id"]?.ToString();

        if (!Guid.TryParse(businessIdValue, out var businessId))
        {
            return;
        }


        var member =
            await businessMemberRepository.GetByBusinessAndUserIdAsync(
                businessId,
                currentUser.UserId,
                httpContext.RequestAborted);

        if (member is null)
            return;

        if (requirement.AllowedRoles.Contains(member.Role))
            context.Succeed(requirement);
    }
}