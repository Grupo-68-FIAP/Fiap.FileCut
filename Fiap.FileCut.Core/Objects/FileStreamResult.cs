namespace Fiap.FileCut.Infra.Storage.Shared.Models
{
	public class FileStreamResult : IDisposable
	{
		public string FileName { get; }
		public Stream FileStream { get; }

		public FileStreamResult() { }

		public FileStreamResult(string fileName, Stream fileStream)
		{
			FileName = fileName;
			FileStream = fileStream;
		}

		public void Dispose()
		{
			FileStream?.Dispose();
		}
	}
}