using Fiap.FileCut.Infra.Storage.Shared.Models;
using Microsoft.AspNetCore.Http;

namespace Fiap.FileCut.Core.Interfaces.Services;

public interface IFileService
{
	Task<FileStreamResult> GetFileAsync(Guid userId, string fileName, CancellationToken cancellationToken);
	Task<IList<string>> GetFileNamesAsync(Guid userId, CancellationToken cancellationToken);
	Task<bool> SaveFileAsync(Guid userId, string fileName, Stream fileStream, CancellationToken cancellationToken);
	Task<bool> DeleteFileAsync(Guid userId, string fileName, CancellationToken cancellationToken);
}