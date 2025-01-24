using Fiap.FileCut.Core.Objects;
using Microsoft.AspNetCore.Http;

namespace Fiap.FileCut.Core.Interfaces.Applications;

public interface IGestaoApplication
{
    Task<List<VideoMetadata>> ListAllVideosAsync(Guid guid);

    Task<VideoMetadata> GetVideoMetadataAsync(Guid guid, string videoName);

    Task<IFormFile> GetVideoAsync(Guid guid, string videoName);
}
