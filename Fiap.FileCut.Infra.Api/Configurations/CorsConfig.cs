using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fiap.FileCut.Infra.Api.Configurations
{
    public static class CorsConfig
    {
        public static void AddEnvCors(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                var allowOrigins = configuration.GetValue<string>("ALLOW_ORIGINS") ?? "*";
                options.AddPolicy("CorsConfig", builder => builder.ConfigureCorsPolicy(allowOrigins));
            });
        }

        private static void ConfigureCorsPolicy(this CorsPolicyBuilder builder, string allowOrigins)
        {
            if (allowOrigins == "*")
            {
                builder.AllowAnyOrigin();
            } else
            {
                builder.WithOrigins(allowOrigins.Split(';'));
            }
            builder.AllowAnyMethod().AllowAnyHeader();
        }

        public static void UseEnvCors(this IApplicationBuilder app)
        {
            app.UseCors("CorsPolicy");
        }
    }
}
