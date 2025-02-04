using Fiap.FileCut.Core.Interfaces.Applications;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace Fiap.FileCut.Core.Applications;

public class GestaoApplication(IFileService fileService) : IGestaoApplication
{
	public async Task<IFormFile> GetVideoAsync(Guid guid, string videoName, CancellationToken cancellationToken)
	{
		return await fileService.GetFileAsync(guid, videoName, cancellationToken);
	}

	public async Task<VideoMetadata> GetVideoMetadataAsync(Guid guid, string videoName, CancellationToken cancellationToken)
	{
		var file = await fileService.GetFileAsync(guid, videoName, cancellationToken)
					?? throw new FileLoadException("File not found");

		// TODO NOSONAR: Implementar a leitura do status do vídeo

		return new VideoMetadata(file.FileName);
	}

	public async Task<List<VideoMetadata>> ListAllVideosAsync(Guid guid, CancellationToken cancellationToken)
	{
        var fileNames = await fileService.GetFileNamesAsync(guid, cancellationToken);
		var metadataTasks = fileNames.Select(file => GetVideoMetadataAsync(guid, file, cancellationToken));

		var metadataList = await Task.WhenAll(metadataTasks);

		return metadataList.ToList();
	}
}
