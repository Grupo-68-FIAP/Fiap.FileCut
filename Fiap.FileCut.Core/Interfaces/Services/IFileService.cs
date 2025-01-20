using Microsoft.AspNetCore.Http;

namespace Fiap.FileCut.Core.Interfaces.Services;

public interface IFileService
{
    Task<IFormFile> GetFileAsync(Guid userId, string fileName);

    Task<bool> SaveFileAsync(Guid userId, IFormFile file);

    Task<bool> DeleteFileAsync(Guid userId, string fileName);
}