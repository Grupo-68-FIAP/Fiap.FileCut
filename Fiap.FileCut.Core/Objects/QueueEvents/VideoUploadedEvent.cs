using Fiap.FileCut.Core.Attributes;
using Fiap.FileCut.Core.Objects.Enums;

namespace Fiap.FileCut.Core.Objects.QueueEvents;

[MessageQueue(Queues.PROCESS)]
public class VideoUploadedEvent(string videoName) : BaseMessageEvent
{
    public string VideoName { get; set; } = videoName;
}
