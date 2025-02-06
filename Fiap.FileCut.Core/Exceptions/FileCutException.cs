namespace Fiap.FileCut.Core.Exceptions;

public class FileCutException : Exception
{
    public FileCutException(string message) : base(message) { }
    public FileCutException(string message, Exception innerException) : base(message, innerException) { }
}
