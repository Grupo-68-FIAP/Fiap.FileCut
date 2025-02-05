namespace Fiap.FileCut.Core.Interfaces.Services;

public interface IVideoProcessingService
{
    Task ProcessVideoAsync(Guid userId, string videoPath);
}
