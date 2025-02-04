namespace Fiap.FileCut.Infra.Api.Configurations
{
	public class S3Config
	{
		public string? Region { get; set; }
		public string? AccessKey { get; set; }
		public string? SecretKey { get; set; }
		public string? BucketName { get; set; }
	}
}