namespace Fiap.FileCut.Core.Interfaces.Applications;

public interface IUploadApplication
{
    Task<bool> UploadFileAsync(Guid userId, string fileName, Stream fileStream, CancellationToken cancellationToken);   
}
