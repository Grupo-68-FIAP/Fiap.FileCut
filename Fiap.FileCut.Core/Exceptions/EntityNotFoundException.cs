namespace Fiap.FileCut.Core.Exceptions;

public class EntityNotFoundException : FileCutException
{
    public EntityNotFoundException(string message) : base(message) { }
    public EntityNotFoundException(string message, Exception innerException) : base(message, innerException) { }
}
