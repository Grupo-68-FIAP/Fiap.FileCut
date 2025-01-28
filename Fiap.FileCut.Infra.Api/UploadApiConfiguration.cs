using Fiap.FileCut.Core.Handler;
using Fiap.FileCut.Infra.Api.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiap.FileCut.Infra.Api
{
    public static class UploadApiConfiguration
    {
        public async static Task ConfigureFileCutUploadApi(this IServiceCollection services)
        {
            services.AddJwtBearerAuthentication();
            services.AddSwaggerFC();
            services.AddEnvCors();
            await services.AddQueue(cfg =>
            {
                cfg.SubscribeQueue<string, TestHandler>("test-queue");
            });
        }

        public async static Task ScopedFileCutUploadApi(this IServiceScope scope)
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
