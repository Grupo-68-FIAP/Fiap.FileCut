using Fiap.FileCut.Core.Interfaces.Applications;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;
using Microsoft.AspNetCore.Http;

namespace Fiap.FileCut.Core.Applications;

public class GestaoApplication(IFileService fileService) : IGestaoApplication
{
    public async Task<IFormFile> GetVideoAsync(Guid guid, string videoName)
    {
        return await fileService.GetFileAsync(guid, videoName);
    }

    public async Task<VideoMetadata> GetVideoMetadataAsync(Guid guid, string videoName)
    {
        var file = await fileService.GetFileAsync(guid, videoName)
            ?? throw new FileLoadException("File not found");

        // TODO NOSONAR: Implementar a leitura do status do vídeo

        return new VideoMetadata(file.FileName);
    }

    public async Task<List<VideoMetadata>> ListAllVideosAsync(Guid guid)
    {
        var list = await fileService.GetAllFilesName(guid);

        var tasks = list.Select(file => GetVideoMetadataAsync(guid, file));

        return [.. await Task.WhenAll(tasks)];
    }
}
