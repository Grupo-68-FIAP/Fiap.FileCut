using Fiap.FileCut.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiap.FileCut.Processing.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVideoProcessing(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<VideoProcessingConfig>(
                configuration.GetSection(VideoProcessingConfig.SectionName));

            services.AddSingleton<IVideoProcessingService, VideoProcessingService>();

            return services;
        }
    }
}
