using Fiap.FileCut.Core.Interfaces.Handlers;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;
using Fiap.FileCut.Core.Objects.QueueEvents;
using Microsoft.Extensions.Logging;

namespace Fiap.FileCut.Core.Handlers;

public class VideoProcessorConsumer(
    INotifyService notifyService,
    ILogger<VideoProcessorConsumer> logger,
    IVideoProcessingService videoProcessingService, 
    IPackageService packageService) : IConsumerHandler<VideoUploadedEvent>
{
    public async Task HandleAsync(NotifyContext<VideoUploadedEvent> context)
    {
        logger.LogInformation("Video em processamento");
        UserNotifyEvent evt;
        try
        {
            string zipFilePath = await videoProcessingService.ProcessVideoAsync(context.UserId, context.Value.VideoName);
            logger.LogInformation("Video processado com sucesso");

            // Empacotamento das imagens
            await packageService.PackageImagesAsync(zipFilePath);
            logger.LogInformation("Imagens empacotadas");

            evt = new UserNotifyEvent(context.Value.VideoName)
            {
                PackName = Path.GetFileName(zipFilePath),
                IsSuccess = true
            };

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao processar vídeo");
            evt = new UserNotifyEvent(context.Value.VideoName)
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }

        await notifyService.NotifyAsync(new NotifyContext<UserNotifyEvent>(evt, context.UserId));
    }
}
