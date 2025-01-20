using Microsoft.AspNetCore.Http;

namespace Fiap.FileCut.Core.Interfaces.Repository;

public interface IFileRepository
{
    Task<IFormFile> GetAsync(Guid userId, string fileName);
    Task<IList<IFormFile>> GetAllAsync(Guid userId);
    Task<bool> UpdateAsync(Guid userId, IFormFile file);
    Task<bool> DeleteAsync(Guid userId, string fileName);
}