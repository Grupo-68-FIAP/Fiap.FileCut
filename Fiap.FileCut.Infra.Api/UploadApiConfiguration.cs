using Fiap.FileCut.Core.Handler;
using Fiap.FileCut.Core.Objects;
using Fiap.FileCut.Infra.Api.Configurations;
using Fiap.FileCut.Infra.Api.Enums;
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
            services.AddNotifications()
                .MessagingPublisherNotify();

            await services.AddQueue(cfg =>
            {
                //cfg.SubscribeQueue<string, TestHandler>(MessageQueues.INFORMATION);
                cfg.SubscribeQueue<NotifyContext<InformationMessageEvents>, InformationHandler>(MessageQueues.INFORMATION);
                //cfg.SubscribeQueue<string, TestHandler>(MessageQueues.PROCESS);
                //cfg.SubscribeQueue<string, TestHandler>(MessageQueues.PACK);
            });
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
            return Task.CompletedTask;
        }
    }
}
