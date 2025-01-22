using Fiap.FileCut.Core.Interfaces.Repository;
using Microsoft.AspNetCore.Http;

namespace Fiap.FileCut.Infra.Storage.LocalDisk;

public class LocalDiskFileRepository : IFileRepository
{

    private string _localStorageFolderPath;

    public LocalDiskFileRepository()
    {
        _localStorageFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "LocalStorage");
        CreateDirectory(_localStorageFolderPath);
    }

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

    public async Task<bool> UpdateAsync(Guid userId, IFormFile file)
    {
        if (file == null)
            throw new ArgumentNullException(nameof(file));

        if (file.Length > 0)
        {
            string filePath = Path.Combine(_localStorageFolderPath, userId.ToString());
            CreateDirectory(filePath);
            filePath = Path.Combine(filePath, file.Name);

            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
                return File.Exists(filePath);
            }
        }

        return false;
    }

    private DirectoryInfo CreateDirectory(string path) {
        return Directory.CreateDirectory(path);
    }
}