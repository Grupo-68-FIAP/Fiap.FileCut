using Fiap.FileCut.Core.Interfaces.Handlers;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;
using Microsoft.Extensions.Logging;

namespace Fiap.FileCut.Core.Handlers;

public class StatusUpdateHandler(
    INotifyService notifyService,
    IUserService userService,
    ILogger<StatusUpdateHandler> logger) : IMessageHandler<string>
{
    public async Task HandleAsync(NotifyContext<string> context)
    {
        logger.LogDebug("Status do usuario {UserId} atualizado: {Status}", context.UserId, context.Context);

        var user = await userService.GetUserAsync(context.UserId)
            ?? throw new InvalidOperationException("User not found");

        var newContext = new FileCutMailMessage(user.Email)
        {
            Subject = "Status atualizado",
            Body = $"Status atualizado para: {context.Context}"
        };
        await notifyService.NotifyAsync(new NotifyContext<FileCutMailMessage>(newContext, context.UserId));
    }
}