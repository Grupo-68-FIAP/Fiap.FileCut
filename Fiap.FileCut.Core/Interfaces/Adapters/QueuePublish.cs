using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;

namespace Fiap.FileCut.Core.Interfaces.Adapters
{
    public abstract class QueuePublish<T> : INotifyAdapter where T : class
    {
        private readonly IMessagingPublisherService messagingPublisherService;

        protected QueuePublish(IMessagingPublisherService messagingPublisherService)
        {
            this.messagingPublisherService = messagingPublisherService;
        }

        public async Task NotifyAsync<T1>(NotifyContext<T1> notifyContext)
        {
            if (notifyContext.Context is T t)
                await PublishAsync(notifyContext.Convert(t));
        }

        public virtual async Task PublishAsync(NotifyContext<T> notifyContext)
        {
            await messagingPublisherService.PublishMessage(notifyContext);
        }
    }
}
