
using Microsoft.AspNetCore.Authorization;
using Yuggenda.Application.Abstractions.Authentication;
using Yuggenda.Application.Abstractions.Persistence;

namespace Yuggenda.Api.Authorization;

public sealed class BusinessPermissionHandler(
    IBusinessMemberRepository businessMemberRepository,
    ICurrentUser currentUser,
    IHttpContextAccessor httpContextAccessor)
    : AuthorizationHandler<BusinessPermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        BusinessPermissionRequirement requirement)
    {
        if (context.Resource is not Guid businessId)
            return;

        var member =
            await businessMemberRepository.GetByBusinessAndUserIdAsync(
                businessId,
                currentUser.UserId,
                httpContextAccessor.HttpContext?.RequestAborted
                    ?? CancellationToken.None);

        if (member is not null &&
            requirement.AllowedRoles.Contains(member.Role))
        {
            context.Succeed(requirement);
        }
    }
}
