using Fiap.FileCut.Core.Attributes;
using Fiap.FileCut.Core.Enums;

namespace Fiap.FileCut.Core.Objects;

[MessageQueue(Queues.PROCESS)]
public class ProcessMessageEvent : BaseMessageEvent
{
    public string Message { get; set; }
}
