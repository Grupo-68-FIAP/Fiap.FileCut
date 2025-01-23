using Fiap.FileCut.Core.Interfaces.Handler;
using Microsoft.Extensions.Logging;

namespace Fiap.FileCut.Core.Handler;

public class TestHandler(ILogger<TestHandler> logger) : IMessageHandler<string>
{
    private readonly ILogger<TestHandler> _logger = logger;

    public async Task HandleAsync(string message)
    {
        _logger.LogInformation("Test: {Message}", message);

        await Task.Delay(1000);
    }
}
