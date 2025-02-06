using Fiap.FileCut.Core.Exceptions;
using Fiap.FileCut.Core.Interfaces.Applications;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;
using Fiap.FileCut.Infra.Storage.Shared.Models;

namespace Fiap.FileCut.Core.Applications;

public class GestaoApplication(IFileService fileService) : IGestaoApplication
{
    public async Task<FileStreamResult> GetVideoAsync(Guid guid, string videoName, CancellationToken cancellationToken)
    {
        try
        {
            return await fileService.GetFileAsync(guid, videoName, cancellationToken);
        }
        catch (FileNotFoundException ex)
        {
            throw new EntityNotFoundException("File not found", ex);
        }
    }

    public async Task<VideoMetadata> GetVideoMetadataAsync(Guid guid, string videoName, CancellationToken cancellationToken)
    {
        try
        {
            FileStreamResult file = await fileService.GetFileAsync(guid, videoName, cancellationToken);
            VideoState videoState = await ReadVideoState(guid, videoName, cancellationToken);
            return new VideoMetadata(file.FileName, videoState);
        }
        catch (FileNotFoundException ex)
        {
            throw new EntityNotFoundException("File not found", ex);
        }
    }

    private async Task<VideoState> ReadVideoState(Guid guid, string videoName, CancellationToken cancellationToken)
    {
        try
        {
            FileStreamResult status = await fileService.GetFileAsync(guid, $"{videoName}.state", cancellationToken);
            string state = await new StreamReader(status.FileStream).ReadToEndAsync(cancellationToken);
            VideoState videoState = Enum.Parse<VideoState>(state);
            return videoState;
        }
        catch (FileNotFoundException)
        {
            return VideoState.PENDING;
        }
    }

    public async Task<List<VideoMetadata>> ListAllVideosAsync(Guid guid, CancellationToken cancellationToken)
    {
        var fileNames = await fileService.GetFileNamesAsync(guid, cancellationToken);
        var metadataTasks = fileNames
            .Where(file => !file.EndsWith(".state"))
            .Select(file => GetVideoMetadataAsync(guid, file, cancellationToken));

        var metadataList = await Task.WhenAll(metadataTasks);

        return [.. metadataList];
    }
}
