using Fiap.FileCut.Core.Adapters;
using Fiap.FileCut.Core.Applications;
using Fiap.FileCut.Core.Interfaces.Applications;
using Fiap.FileCut.Core.Objects.QueueEvents;
using Fiap.FileCut.Infra.Api.Configurations;
using Fiap.FileCut.Infra.Api.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Fiap.FileCut.Infra.Api
{
    public static class UploadApiConfiguration
    {
        public static async Task ConfigureFileCutUploadApi(this IServiceCollection services)
        {
            services.AddJwtBearerAuthentication();
            services.AddSwaggerFC();
            services.AddEnvCors();
            services.AddNotifications();

            await services.AddQueue(cfg =>
            {
                cfg.AddPublisher<VideoUploadedEvent, VideoUploadedQueuePublish>();
                cfg.AddPublisher<UserNotifyEvent, UserNotifyQueuePublish>();
            });
            services.AddScoped<IUploadApplication, UploadApplication>();
            services.AddStorageService();
        }

        public static async Task ScopedFileCutUploadApi(this IServiceScope scope)
        {
            await scope.UseQueue();
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
