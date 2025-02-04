using Fiap.FileCut.Core.Attributes;
using Fiap.FileCut.Core.Enums;

namespace Fiap.FileCut.Core.Objects;

[MessageQueue(Queues.PACK)]
public class PackMessageEvent : BaseMessageEvent
{
    public string Message { get; set; }
}
