using Fiap.FileCut.Core.Interfaces.Repository;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Services;
using Fiap.FileCut.Infra.IdentityProvider.Keycloak;
using Fiap.FileCut.Infra.IdentityProvider;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Fiap.FileCut.Infra.IdentityProvider.Keycloak.Objects;

namespace Fiap.FileCut.Infra.Api.Configurations;

/// <summary>
/// OpenId configuration class.
/// </summary>
public static class OpenIdConfig
{
    /// <summary>
    /// Add OpenIdConnect configuration to the application.
    /// </summary>
    /// <param name="services">ServiceCollection instance.</param>
    public static void AddJwtBearerAuthentication(this IServiceCollection services)
    {
        var configuration = new KeycloakConfiguration();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options =>
           {
               options.Authority = configuration.Authority;
               options.Audience = configuration.ClientId;
               options.RequireHttpsMetadata = false;
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidIssuer = configuration.Authority,
                   ValidAudience = configuration.ClientId,
                   ValidateIssuer = true,
                   ValidateAudience = false,
                   ValidateLifetime = true,
               };
           });

        services.AddMemoryCache();
        services.AddHttpContextAccessor();
        services.AddHttpClient();
        services.AddSingleton(c => configuration);
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserRepository, KeycloakUserRepository>();
    }

    /// <summary>
    /// Add OpenIdConnect configuration to the application.
    /// </summary>
    /// <param name="app">WebApplication instance.</param>
    public static void UseAuth(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}
