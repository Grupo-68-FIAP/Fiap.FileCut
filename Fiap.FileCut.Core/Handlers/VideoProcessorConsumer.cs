using Fiap.FileCut.Core.Interfaces.Handlers;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;
using Fiap.FileCut.Core.Objects.Enums;
using Fiap.FileCut.Core.Objects.QueueEvents;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Fiap.FileCut.Core.Handlers;

public class VideoProcessorConsumer(
    IFileService fileService,
    INotifyService notifyService,
    IVideoProcessingService videoProcessingService,
    ILogger<VideoProcessorConsumer> logger) : IConsumerHandler<VideoUploadedEvent>
{
    public async Task HandleAsync(NotifyContext<VideoUploadedEvent> context)
    {
        logger.LogInformation("Video em processamento");
        UserNotifyEvent evt;
        try
        {
            await StateUpdate(context, VideoState.PROCESSING ,fileService);

            var video = await fileService.GetFileAsync(context.UserId, context.Value.VideoName, CancellationToken.None);

            var zipstream = await videoProcessingService.ProcessVideoAsync(video.FileStream);

            var zipName = Path.ChangeExtension(context.Value.VideoName, ".zip");

            await fileService.SaveFileAsync(context.UserId, zipName, zipstream, CancellationToken.None);

            evt = new UserNotifyEvent(context.Value.VideoName)
            {
                PackName = Path.GetFileName(zipName),
                IsSuccess = true
            };

            await StateUpdate(context, VideoState.FINISH, fileService);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao processar vídeo");
            evt = new UserNotifyEvent(context.Value.VideoName)
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };

            await StateUpdate(context, VideoState.FAILED, fileService);
        }

        await notifyService.NotifyAsync(new NotifyContext<UserNotifyEvent>(evt, context.UserId));
    }

    private static async Task StateUpdate<T>(NotifyContext<T> context, VideoState state, IFileService fileService) where T : VideoUploadedEvent
    {
        var fileState = Path.ChangeExtension(context.Value.VideoName, ".state");
        var stateStream = new MemoryStream(Encoding.UTF8.GetBytes(state.ToString()));
        await fileService.SaveFileAsync(context.UserId, fileState, stateStream, CancellationToken.None);
    }
}
