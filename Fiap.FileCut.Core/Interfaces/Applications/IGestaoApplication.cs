using Fiap.FileCut.Core.Objects;
using Fiap.FileCut.Infra.Storage.Shared.Models;
using Microsoft.AspNetCore.Http;

namespace Fiap.FileCut.Core.Interfaces.Applications;

public interface IGestaoApplication
{
    Task<List<VideoMetadata>> ListAllVideosAsync(Guid guid, CancellationToken cancellationToken);
    Task<VideoMetadata> GetVideoMetadataAsync(Guid guid, string videoName, CancellationToken cancellationToken);
    Task<FileStreamResult> GetVideoAsync(Guid guid, string videoName, CancellationToken cancellationToken);
}
