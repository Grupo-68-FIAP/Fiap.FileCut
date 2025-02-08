using Fiap.FileCut.Core.Adapters;
using Fiap.FileCut.Core.Handlers;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects.QueueEvents;
using Fiap.FileCut.Infra.Api.Configurations;
using Fiap.FileCut.Infra.Api.Middlewares;
using Fiap.FileCut.Processing.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Fiap.FileCut.Infra.Api
{
    public static class ProcessamentoApiConfiguration
    {
        public static async Task ConfigureFileCutProcessamentoApi(this WebApplicationBuilder builder)
        {
            builder.Services.AddJwtBearerAuthentication();
            builder.Services.AddSwaggerFC();
            builder.Services.AddEnvCors();
            builder.Services.AddNotifications();

            await builder.Services.AddQueue(cfg =>
            {
                cfg.SubscribeQueue<VideoUploadedEvent, VideoProcessorConsumer>();
                cfg.AddPublisher<UserNotifyEvent, UserNotifyQueuePublish>();
            });

            builder.Services.AddStorageService();
            builder.Services.AddSingleton<IVideoProcessingService, VideoProcessingService>();
        }

        public static async Task ScopedFileCutProcessamentoApi(this IServiceScope scope)
        {
            await scope.UseQueue();
        }

        public static Task InitializeFileCutProcessamentoApi(this IApplicationBuilder app)
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
