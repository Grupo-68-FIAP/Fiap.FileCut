using Fiap.FileCut.Infra.Api.Configurations;
using Fiap.FileCut.Infra.Api.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Fiap.FileCut.Infra.Api
{
    public static class UploadApiConfiguration
    {
        public static Task ConfigureFileCutUploadApi(this IServiceCollection services)
        {
            services.AddJwtBearerAuthentication();
            services.AddSwaggerFC();
            services.AddEnvCors();

            return Task.CompletedTask;
        }

        public static Task ScopedFileCutUploadApi(this IServiceScope scope)
        {
            return Task.CompletedTask;
        }

        public static Task InitializeFileCutUploadApi(this IApplicationBuilder app)
        {
            app.UseSwaggerFC();
            app.UseEnvCors();
            app.UseHttpsRedirection();
            app.UseAuth();
            app.UseMiddleware<ErrorHandlerMiddleware>();
            return Task.CompletedTask;
        }
    }
}
