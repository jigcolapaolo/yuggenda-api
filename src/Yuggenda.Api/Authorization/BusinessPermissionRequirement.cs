
using Microsoft.AspNetCore.Authorization;
using Yuggenda.Domain.Enums;

namespace Yuggenda.Api.Authorization;

public sealed class BusinessPermissionRequirement(
    params BusinessRole[] allowedRoles) : IAuthorizationRequirement
{
    public IReadOnlyCollection<BusinessRole> AllowedRoles { get; } = allowedRoles;
}