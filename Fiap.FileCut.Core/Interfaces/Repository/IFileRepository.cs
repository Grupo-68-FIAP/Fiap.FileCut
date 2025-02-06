using Fiap.FileCut.Infra.Storage.Shared.Models;

namespace Fiap.FileCut.Core.Interfaces.Repository;

public interface IFileRepository
{
	Task<FileStreamResult?> GetAsync(Guid userId, string fileName, CancellationToken cancellationToken);
	Task<IList<FileStreamResult>> GetAllAsync(Guid userId, CancellationToken cancellationToken);
	Task<IList<string>> ListFileNamesAsync(Guid userId, CancellationToken cancellationToken = default);
	Task<bool> UpdateAsync(Guid userId, Stream fileStream, string fileName, CancellationToken cancellationToken);
	Task<bool> DeleteAsync(Guid userId, string fileName, CancellationToken cancellationToken);
}