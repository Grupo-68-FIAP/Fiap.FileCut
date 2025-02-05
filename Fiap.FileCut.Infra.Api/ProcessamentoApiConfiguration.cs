using Fiap.FileCut.Core.Handlers;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;
using Fiap.FileCut.Infra.Api.Configurations;
using Fiap.FileCut.Processing.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Fiap.FileCut.Infra.Api
{
    public static class ProcessamentoApiConfiguration
    {
        public async static Task ConfigureFileCutProcessamentoApi(this WebApplicationBuilder builder)
        {
            builder.Services.AddJwtBearerAuthentication();
            builder.Services.AddSwaggerFC();
            builder.Services.AddEnvCors();
            await builder.Services.AddQueue(cfg =>
            {
                cfg.SubscribeQueue<string, VideoProcessingHandler>();
            });


            builder.Services.AddScoped<IVideoProcessingService, VideoProcessingService>();
            builder.Services.AddScoped<ProcessingOptions>();
        }

        public static Task ScopedFileCutProcessamentoApi(this IServiceScope scope)
        {
            return Task.CompletedTask;
        }

        public static Task InitializeFileCutProcessamentoApi(this IApplicationBuilder app)
        {
            app.UseSwaggerFC();
            app.UseEnvCors();
            app.UseHttpsRedirection();
            app.UseAuth();
            return Task.CompletedTask;
        }
    }
}
