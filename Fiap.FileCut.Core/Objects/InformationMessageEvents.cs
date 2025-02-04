using Fiap.FileCut.Core.Attributes;
using Fiap.FileCut.Core.Enums;

namespace Fiap.FileCut.Core.Objects;

[MessageQueue(Queues.INFORMATION)]
public class InformationMessageEvents : BaseMessageEvent
{
    public string Status { get; set; }
    public string Message { get; set; }
}