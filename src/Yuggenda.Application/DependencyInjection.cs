using Microsoft.Extensions.DependencyInjection;
using Yuggenda.Application.Services.Authentication;

namespace Yuggenda.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<UserRegistrationService>();
        services.AddScoped<UserLoginService>();

        return services;
    }
}