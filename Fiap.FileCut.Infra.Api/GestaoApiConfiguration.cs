using Fiap.FileCut.Core.Applications;
using Fiap.FileCut.Core.Interfaces.Applications;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Services;
using Fiap.FileCut.Infra.Api.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Fiap.FileCut.Infra.Api;

public static class GestaoApiConfiguration
{
    public static Task ConfigureFileCutGestaoApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddJwtBearerAuthentication();
        builder.Services.AddSwaggerFC();
        builder.Services.AddEnvCors();
        builder.Services.AddNotifications()
                .EmailNotify(builder.Configuration);

        builder.Services.AddScoped<IFileService, FileService>();
        builder.Services.AddScoped<IGestaoApplication, GestaoApplication>();

        return Task.CompletedTask;
    }
    public static Task ScopedFileCutGestaoApi(this IServiceScope scope)
    {
        return Task.CompletedTask;
    }

    public static Task InitializeFileCutGestaoApi(this IApplicationBuilder app)
    {
        app.UseSwaggerFC();
        app.UseEnvCors();
        app.UseHttpsRedirection();
        app.UseAuth();
        return Task.CompletedTask;
    }
}
