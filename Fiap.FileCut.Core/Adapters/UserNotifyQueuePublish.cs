using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects.QueueEvents;

namespace Fiap.FileCut.Core.Adapters;

public class UserNotifyQueuePublish(IMessagingPublisherService messagingPublisherService)
    : QueuePublish<UserNotifyEvent>(messagingPublisherService)
{

}
