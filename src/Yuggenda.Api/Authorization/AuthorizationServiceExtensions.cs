using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Yuggenda.Domain.Enums;

namespace Yuggenda.Api.Authorization;

public static class AuthorizationServiceExtensions
{
    public static IServiceCollection AddYuggendaAuthorization(
        this IServiceCollection services
    )
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(BusinessPolicies.Update, policy =>
            {
                policy.RequireAuthenticatedUser();

                policy.AddRequirements(
                    new BusinessPermissionRequirement(
                        BusinessRole.Owner,
                        BusinessRole.Admin)
                    );
            });

            options.AddPolicy(BusinessPolicies.Delete, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.AddRequirements(
                    new BusinessPermissionRequirement(
                        BusinessRole.Owner
                    )
                );
            });

            options.AddPolicy(BusinessPolicies.ReadMembers, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.AddRequirements(
                    new BusinessPermissionRequirement(
                        BusinessRole.Owner,
                        BusinessRole.Admin,
                        BusinessRole.Staff
                    )
                );
            });

            options.AddPolicy(BusinessPolicies.AddMembers, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.AddRequirements(
                    new BusinessPermissionRequirement(
                        BusinessRole.Owner,
                        BusinessRole.Admin
                    )
                );
            });

            
            options.AddPolicy(BusinessPolicies.UpdateMemberRole, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.AddRequirements(
                    new BusinessPermissionRequirement(BusinessRole.Owner));
            });

        });

        return services;
    }
}