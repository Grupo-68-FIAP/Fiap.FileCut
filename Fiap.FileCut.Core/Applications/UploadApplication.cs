using Fiap.FileCut.Core.Interfaces.Applications;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;
using Fiap.FileCut.Core.Objects.QueueEvents;

namespace Fiap.FileCut.Core.Applications;

public class UploadApplication(
    INotifyService notifyService,
    IFileService fileService) : IUploadApplication
{
    public async Task<bool> UploadFileAsync(Guid userId, string fileName, Stream fileStream, CancellationToken cancellationToken)
    {
        var message = $"Uploading file {fileName} for user {userId} is not possible";  
        try
        {
            bool uploaded = await fileService.SaveFileAsync(userId, fileName, fileStream, CancellationToken.None);
            if (uploaded)
            {
                var uploadEvent = new VideoUploadedEvent(fileName);
                await notifyService.NotifyAsync(new NotifyContext<VideoUploadedEvent>(uploadEvent, userId));
                return uploaded;
            }
        }
        catch (Exception ex)
        {
            message = $"Error uploading file {fileName}:\n {ex.Message}";
        }

        var userEvent = new UserNotifyEvent(fileName)
        {
            IsSuccess = false,
            ErrorMessage = message
        };
        await notifyService.NotifyAsync(new NotifyContext<UserNotifyEvent>(userEvent, userId));

        return false;
    }
}
