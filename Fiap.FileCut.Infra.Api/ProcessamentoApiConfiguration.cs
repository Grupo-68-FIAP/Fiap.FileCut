using Fiap.FileCut.Infra.Api.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Fiap.FileCut.Infra.Api
{
    public static class ProcessamentoApiConfiguration
    {
        public static Task ConfigureFileCutProcessamentoApi(this WebApplicationBuilder builder)
        {
            builder.Services.AddJwtBearerAuthentication();
            builder.Services.AddSwaggerFC();
            builder.Services.AddEnvCors();

            return Task.CompletedTask;
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
