using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace Fiap.FileCut.Infra.Storage.Shared.Helpers
{
	public static class FileHelper
	{
		public static bool IsValidFileName(string fileName)
		{
			return !string.IsNullOrWhiteSpace(fileName) && fileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
		}
	}
}
