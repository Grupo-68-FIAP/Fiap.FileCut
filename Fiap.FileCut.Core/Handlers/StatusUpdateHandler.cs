using Fiap.FileCut.Core.Interfaces.Handlers;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;
using Microsoft.Extensions.Logging;

namespace Fiap.FileCut.Core.Handlers;

public class StatusUpdateHandler(
    INotifyService notifyService,
    ILogger<StatusUpdateHandler> logger) : IMessageHandler<string>
{
    private readonly ILogger<StatusUpdateHandler> _logger = logger;
    private readonly INotifyService _notifyService = notifyService;

    public async Task HandleAsync(NotifyContext<string> context)
    {
        _logger.LogDebug("Status do usuario {UserId} atualizado: {Status}", context.UserId, context.Context);

        // TODO NOSONAR: É nescessario pegar o email no keycloak
        var newContext = new FileCutMailMessage("tesst@test.com") {
            Subject = "Status atualizado",
            Body = $"Status atualizado para: {context.Context}"
        };
        await _notifyService.NotifyAsync(new NotifyContext<FileCutMailMessage>(newContext, context.UserId));
    }
}