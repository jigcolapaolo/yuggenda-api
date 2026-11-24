using Yuggenda.Application;
using Yuggenda.Infrastructure;
using Yuggenda.Infrastructure.Persistence.Context;
using Yuggenda.Infrastructure.Persistence.Seed;
using Microsoft.OpenApi.Models;
using Yuggenda.Api.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Yuggenda.Infrastructure.Authentication;
using Yuggenda.Application.Abstractions.Authentication;
using Yuggenda.Api.Authentication;
using Yuggenda.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Yuggenda.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Yuggenda API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT access token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter()
        );
    });

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICurrentUser, CurrentUser>();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks()
    .AddDbContextCheck<YuggendaDbContext>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        var jwtOptions = new JwtOptions();

        builder.Configuration
            .GetSection("Jwt")
            .Bind(jwtOptions);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

            options.Events = new JwtBearerEvents
            {
                OnChallenge = async context =>
                {
                    // Evita que ASP.NET Core genere su respuesta predeterminada.
                    context.HandleResponse();

                    var problem = new ProblemDetails
                    {
                        Type = "about:blank",
                        Title = "Unauthorized",
                        Status = StatusCodes.Status401Unauthorized,
                        Detail = "Authentication is required or the access token is invalid."
                    };

                    problem.Extensions["requestId"] =
                        context.HttpContext.TraceIdentifier;

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                    context.Response.ContentType = "application/problem+json";

                    await context.Response.WriteAsJsonAsync(problem);
                }
            };
    });

builder.Services.AddYuggendaAuthorization();

builder.Services.AddScoped<IAuthorizationHandler, BusinessPermissionHandler>();


var app = builder.Build();

app.UseMiddleware<RequestIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var context = scope.ServiceProvider
        .GetRequiredService<YuggendaDbContext>();

    await DevelopmentDataSeeder.SeedAsync(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
