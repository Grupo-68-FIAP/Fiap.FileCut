using Fiap.FileCut.Core.Interfaces.Repository;
using Fiap.FileCut.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace Fiap.FileCut.Core.Services;

public class FileService : IFileService
{
    private readonly IFileRepository _fileRepository;
    public FileService(
        IFileRepository fileRepository
    )
    {
        _fileRepository = fileRepository;
    }
    public Task<bool> DeleteFileAsync(Guid userId, string fileName)
    {
        throw new NotImplementedException();
    }

    public Task<IFormFile> GetFileAsync(Guid userId, string fileName)
    {
        return _fileRepository.GetAsync(userId, fileName);
    }

    public Task<bool> SaveFileAsync(Guid userId, IFormFile file)
    {
        if (file.Length <= 0)
            throw new ArgumentException("Tamanho do arquivo inválido");

        //TODO: Fazer as demais regras aqui

        throw new NotImplementedException();
    }
}
