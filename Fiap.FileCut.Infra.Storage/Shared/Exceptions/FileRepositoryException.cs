namespace Fiap.FileCut.Infra.Storage.Shared.Exceptions
{
	public class FileRepositoryException : Exception
	{
		public FileRepositoryException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}