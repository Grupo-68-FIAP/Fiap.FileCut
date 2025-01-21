using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Fiap.FileCut.Infra.Api.Configurations
{
    public static class SwaggerConfig
    {
        public static void AddSwaggerFC(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                var openIdAuthority = Environment.GetEnvironmentVariable("OPENID_AUTHORITY");
                var openIdTokenUrl = Environment.GetEnvironmentVariable("OPENID_URL_TOKEN") 
                    ?? $"{openIdAuthority}/protocol/openid-connect/token";
                var openIdAuthorization = Environment.GetEnvironmentVariable("OPENID_URL_AUTHORIZATION") 
                    ?? $"{openIdAuthority}/protocol/openid-connect/auth";

                if (string.IsNullOrEmpty(openIdAuthority))
                {
                    return;
                }

                c.AddSecurityDefinition("Bearer",
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri(openIdTokenUrl),
                            AuthorizationUrl = new Uri(openIdAuthorization)
                        },
                        Password = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri(openIdTokenUrl),
                            AuthorizationUrl = new Uri(openIdAuthorization)
                        }
                    }
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
                        Array.Empty<string>()
                    }
                });
            });
        }

        public static void UseSwaggerFC(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                var openIdAudance = Environment.GetEnvironmentVariable("OPENID_AUDIENCE");
                if (!string.IsNullOrEmpty(openIdAudance))
                {
                    c.OAuthClientId(openIdAudance);
                }
            });
        }
    }
}
