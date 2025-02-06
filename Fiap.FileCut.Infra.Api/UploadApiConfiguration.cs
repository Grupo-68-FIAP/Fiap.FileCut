using Fiap.FileCut.Core.Adapters;
using Fiap.FileCut.Core.Objects.QueueEvents;
using Fiap.FileCut.Infra.Api.Configurations;
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
            return Task.CompletedTask;
        }
    }
}
