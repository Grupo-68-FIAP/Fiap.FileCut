using Fiap.FileCut.Core.Objects;

namespace Fiap.FileCut.Core.Interfaces.Services
{
    public interface IMessagingPublisherService
    {

        Task PublishMessage<T>(NotifyContext<T> context);
    }
}
