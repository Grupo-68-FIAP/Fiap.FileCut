using Fiap.FileCut.Infra.Api.Configurations;
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
            //await services.AddQueue(cfg =>
            //{
            //    cfg.SubscribeQueue<string, TestHandler>("test-queue");
            //});

            return Task.CompletedTask;
        }

        public static Task ScopedFileCutUploadApi(this IServiceScope scope)
        {
            //await scope.UseQueue();
            return Task.CompletedTask;
        }

        public static Task InitializeFileCutUploadApi(this IApplicationBuilder app)
        {
            app.UseSwaggerFC();
            app.UseEnvCors();
            app.UseHttpsRedirection();
            app.UseAuth();
            return Task.CompletedTask;
        }
    }
}
