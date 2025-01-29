using Fiap.FileCut.Core.Interfaces.Handler;
using Fiap.FileCut.Core.Objects;
using Microsoft.Extensions.Logging;

namespace Fiap.FileCut.Core.Handler;

public class TestHandler(ILogger<TestHandler> logger) : IMessageHandler<string>
{
    private readonly ILogger<TestHandler> _logger = logger;

    public async Task HandleAsync(NotifyContext<string> context)
    {
        _logger.LogInformation("Test: {Message}", context.Context);

        await Task.Delay(1000);
    }
}