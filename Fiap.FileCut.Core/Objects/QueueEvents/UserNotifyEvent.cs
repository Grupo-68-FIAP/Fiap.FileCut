using Fiap.FileCut.Core.Attributes;
using Fiap.FileCut.Core.Objects.Enums;

namespace Fiap.FileCut.Core.Objects.QueueEvents;

[MessageQueue(Queues.INFORMATION)]
public class UserNotifyEvent(string videoName) : BaseMessageEvent
{
    public string VideoName { get; set; } = videoName;
    public bool IsSuccess { get; set; } = true;
    public string? ErrorMessage { get; set; }
    public string? PackName {get; set;}
}