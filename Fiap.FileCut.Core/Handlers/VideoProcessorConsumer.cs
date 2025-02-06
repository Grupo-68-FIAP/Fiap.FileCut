using Fiap.FileCut.Core.Interfaces.Handlers;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;
using Fiap.FileCut.Core.Objects.QueueEvents;
using Microsoft.Extensions.Logging;

namespace Fiap.FileCut.Core.Handlers;

public class VideoProcessorConsumer(
    INotifyService notifyService,
    ILogger<VideoProcessorConsumer> logger) : IConsumerHandler<VideoUploadedEvent>
{
    public async Task HandleAsync(NotifyContext<VideoUploadedEvent> context)
    {
        logger.LogInformation("Video em processamento");
        UserNotifyEvent evt;
        try
        {
            //TODO NOSONAR: VANESSA - Implementar processamento do vídeo
            logger.LogInformation("Video processado com sucesso");

            //TODO NOSONAR: VANESSA - Implementar empacotamento das imagens
            logger.LogInformation("Imagens empacotadas");

            evt = new UserNotifyEvent(context.Value.VideoName)
            {
                PackName = "videoExemplo.zip"
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
