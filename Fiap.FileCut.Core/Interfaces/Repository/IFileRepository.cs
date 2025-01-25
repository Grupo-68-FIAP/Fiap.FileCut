using Microsoft.AspNetCore.Http;

namespace Fiap.FileCut.Core.Interfaces.Repository;

public interface IFileRepository
{
    Task<IFormFile> GetAsync(Guid userId, string fileName, CancellationToken cancellationToken);
    Task<IList<IFormFile>> GetAllAsync(Guid userId, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Guid userId, IFormFile file, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid userId, string fileName, CancellationToken cancellationToken);
}