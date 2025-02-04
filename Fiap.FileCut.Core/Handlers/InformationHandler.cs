using Fiap.FileCut.Core.Interfaces.Adapters;
using Fiap.FileCut.Core.Interfaces.Handlers;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;
using Microsoft.Extensions.Logging;

namespace Fiap.FileCut.Core.Handlers;
public class InformationHandler(ILogger<InformationHandler> logger) : IConsumerHandler<InformationMessageEvents>
{
    public async Task HandleAsync(NotifyContext<InformationMessageEvents> message)
    {
        await Task.Delay(1000);
    }
}

public class StringSubscriberHandler() : IConsumerHandler<string>
{
    public async Task HandleAsync(NotifyContext<string> context)
    {
        Console.WriteLine("[SUBSCRIBE] {0}", context.Context);

        await Task.Delay(1000);
    }
}

public class StringPublisherHandler(IMessagingPublisherService service) : INotifyAdapter
{
    public async Task NotifyAsync<T>(NotifyContext<T> context)
    {
        //Console.WriteLine("[PUBLISHER] {0}", context.Context);

        await service.NotifyAsync(context);
    }
}
