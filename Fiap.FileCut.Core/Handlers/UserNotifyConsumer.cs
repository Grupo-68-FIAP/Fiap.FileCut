using Fiap.FileCut.Core.Interfaces.Handlers;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;
using Fiap.FileCut.Core.Objects.QueueEvents;
using Microsoft.Extensions.Logging;

namespace Fiap.FileCut.Core.Handlers;

public class UserNotifyConsumer(
    INotifyService notifyService,
    IUserService userService,
    ILogger<UserNotifyConsumer> logger) : IConsumerHandler<UserNotifyEvent>
{
    public async Task HandleAsync(NotifyContext<UserNotifyEvent> context)
    {
        var message = $"Video {context.Value.VideoName} processado com sucesso, disponível para download em {context.Value.PackName}.";
        var subject = "Vídeo processado com sucesso";
        if (!context.Value.IsSuccess)
        {
            logger.LogWarning("Status do usuario {UserId} atualizado: {Status}", context.UserId, context.Value);
            subject = "Falha ao processar o vídeo";
            message = $"Erro ao processar vídeo: {context.Value.ErrorMessage}";
        }
        else
            logger.LogDebug("Status do usuario {UserId} atualizado: {Status}", context.UserId, context.Value);

        var user = await userService.GetUserAsync(context.UserId)
            ?? throw new InvalidOperationException("User not found");

        var newContext = new FileCutMailMessage(user.Email)
        {
            Subject = subject,
            Body = message
        };
        await notifyService.NotifyAsync(new NotifyContext<FileCutMailMessage>(newContext, context.UserId));
    }
}
