using Amazon.S3;
using Amazon.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Fiap.FileCut.Infra.Storage.S3;
using Fiap.FileCut.Core.Interfaces.Repository;
using Fiap.FileCut.Infra.Storage.Shared.Configs;

namespace Fiap.FileCut.Infra.Storage.Shared.Extensions
{
	//TODO - VALIDAR SE VAI FICAR NO CORE OU NO SERVIÇO QUE USAR
	public static class S3ServiceExtensions
	{
		public static IServiceCollection AddS3Services(this IServiceCollection services, IConfiguration configuration)
		{
			//services.Configure<S3Settings>(configuration.GetSection("AWS"));

			services.AddSingleton<IAmazonS3>(sp =>
			{
				var s3Settings = sp.GetRequiredService<IOptions<S3Settings>>().Value;

				return new AmazonS3Client(
					new BasicAWSCredentials(s3Settings.AccessKey, s3Settings.SecretKey),
					Amazon.RegionEndpoint.GetBySystemName(s3Settings.Region)
				);
			});

			services.AddScoped<IFileRepository, S3FileRepository>();

			return services;
		}
	}
}