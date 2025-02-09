using Fiap.FileCut.Core.Interfaces.Repository;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Services;
using Fiap.FileCut.Infra.Storage.LocalDisk;
using Fiap.FileCut.Infra.Storage.S3;
using Microsoft.Extensions.DependencyInjection;

namespace Fiap.FileCut.Infra.Api.Configurations;

public static class StorageConfig
{
    public static void AddStorageService(this IServiceCollection services)
    {
        var storageProvider = Environment.GetEnvironmentVariable("AWS_BUCKET_NAME");

        if (!String.IsNullOrEmpty(storageProvider))
            services.AddScoped<IFileRepository, S3FileRepository>();
        else
            services.AddScoped<IFileRepository, LocalDiskFileRepository>();

        services.AddScoped<IFileService, FileService>();
    }
}