using Fiap.FileCut.Core.Interfaces.Handler;

namespace Fiap.FileCut.Core.Interfaces.Services;

public interface IMessagingConsumerService
{
    Task SubscribeAsync<T>(string queueName, IMessageHandler<T> handler) where T : class;
}
