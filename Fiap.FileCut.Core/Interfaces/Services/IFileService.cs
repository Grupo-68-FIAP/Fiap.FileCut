using Microsoft.AspNetCore.Http;

namespace Fiap.FileCut.Core.Interfaces.Services;

public interface IFileService
{
    Task<IFormFile> GetFileAsync(Guid userId, string fileName, CancellationToken cancellationToken);
	Task<bool> SaveFileAsync(Guid userId, IFormFile file, CancellationToken cancellationToken);
	Task<bool> DeleteFileAsync(Guid userId, string fileName, CancellationToken cancellationToken);
}