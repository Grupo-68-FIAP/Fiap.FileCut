namespace Fiap.FileCut.Infra.Storage.Shared.Models
{
	public class FileStreamResult : IDisposable
	{
		public string FileName { get; }
		public Stream FileStream { get; }

		private bool _disposed = false;

		public FileStreamResult(string fileName, Stream fileStream)
		{
			FileName = fileName;
			FileStream = fileStream;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					FileStream?.Dispose();
				}
				_disposed = true;
			}
		}

		~FileStreamResult()
		{
			Dispose(false);
		}
	}
}