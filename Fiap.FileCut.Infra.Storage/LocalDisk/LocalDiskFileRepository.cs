using Fiap.FileCut.Core.Interfaces.Repository;
using Microsoft.AspNetCore.Http;

namespace Fiap.FileCut.Infra.Storage.LocalDisk;

public class LocalDiskFileRepository : IFileRepository
{
    public Task<bool> DeleteAsync(Guid userId, string fileName)
    {
        throw new NotImplementedException();
    }

    public Task<IList<IFormFile>> GetAllAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<IFormFile> GetAsync(Guid userId, string fileName)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(Guid userId, IFormFile file)
    {
        throw new NotImplementedException();
    }
}