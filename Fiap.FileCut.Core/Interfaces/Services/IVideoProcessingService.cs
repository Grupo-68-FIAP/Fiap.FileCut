namespace Fiap.FileCut.Core.Interfaces.Services;

public interface IVideoProcessingService
{
        Task<Stream> ProcessVideoAsync(Stream video, CancellationToken cancellationToken = default);
}
