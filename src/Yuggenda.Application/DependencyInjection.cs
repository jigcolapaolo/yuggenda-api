using Microsoft.Extensions.DependencyInjection;
using Yuggenda.Application.Services;
using Yuggenda.Application.Services.Authentication;
using Yuggenda.Application.Services.Availabilities;
using Yuggenda.Application.Services.Businesses;
using Yuggenda.Application.Services.BusinessMembers;
using Yuggenda.Application.Services.Users;

namespace Yuggenda.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<UserRegistrationService>();
        services.AddScoped<UserLoginService>();
        services.AddScoped<UserService>();
        services.AddScoped<RefreshTokenService>();
        services.AddScoped<BusinessService>();
        services.AddScoped<BusinessMemberService>();
        services.AddScoped<ServiceService>();
        services.AddScoped<AvailabilityService>();
        services.AddScoped<AvailabilityCalculationService>();
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}