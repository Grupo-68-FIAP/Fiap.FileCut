namespace Fiap.FileCut.Core.Interfaces.Services;

public interface IVideoProcessingService
{
        Task<string> ProcessVideoAsync(Guid userId, string videoPath, CancellationToken cancellationToken = default);
}
