using Amazon.Runtime;
using Amazon.S3;
using Fiap.FileCut.Core.Interfaces.Repository;
using Fiap.FileCut.Infra.Api.Configurations;
using Fiap.FileCut.Infra.Storage.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Fiap.FileCut.Infra.Storage.Shared.Extensions
{
	public static class S3ServiceConfiguration
	{
		public static IServiceCollection AddS3Services(this IServiceCollection services, IConfiguration configuration)
		{
			// services.Configure<S3Config>(configuration.GetSection("AWS"));

			services.Configure<S3Config>(c => {
				c.AccessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY");
				c.SecretKey = Environment.GetEnvironmentVariable("AWS_SECRET_KEY");
				c.Region = Environment.GetEnvironmentVariable("AWS_REGION");
				c.BucketName = Environment.GetEnvironmentVariable("AWS_BUCKET_NAME");
			});

			services.AddSingleton<IAmazonS3>(sp =>
			{
				var s3Settings = sp.GetRequiredService<IOptions<S3Config>>().Value;

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