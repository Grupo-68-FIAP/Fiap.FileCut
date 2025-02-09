using Fiap.FileCut.Core.Adapters;
using Fiap.FileCut.Core.Applications;
using Fiap.FileCut.Core.Interfaces.Applications;
using Fiap.FileCut.Core.Objects.QueueEvents;
using Fiap.FileCut.Infra.Api.Configurations;
using Fiap.FileCut.Infra.Api.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

namespace Fiap.FileCut.Infra.Api
{
    public static class UploadApiConfiguration
    {
        public static async Task ConfigureFileCutUploadApi(this WebApplicationBuilder builder)
        {
            builder.Services.AddJwtBearerAuthentication();
            builder.Services.AddSwaggerFC();
            builder.Services.AddEnvCors();
            builder.Services.AddNotifications();

            await builder.Services.AddQueue(cfg =>
            {
                cfg.AddPublisher<VideoUploadedEvent, VideoUploadedQueuePublish>();
                cfg.AddPublisher<UserNotifyEvent, UserNotifyQueuePublish>();
            });
            builder.Services.AddScoped<IUploadApplication, UploadApplication>();
            builder.Services.AddStorageService();

            var maxRequestBodySize = Environment.GetEnvironmentVariable("ASPNETCORE_MAX_REQUEST_BODY_SIZE");
            if (long.TryParse(maxRequestBodySize, out var bodySize))
            {
                builder.WebHost.ConfigureKestrel(options =>
                {
                    options.Limits.MaxRequestBodySize = bodySize;
                });

                builder.Services.Configure<FormOptions>(options =>
                {
                    options.MultipartBodyLengthLimit = bodySize;
                });
            }
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
