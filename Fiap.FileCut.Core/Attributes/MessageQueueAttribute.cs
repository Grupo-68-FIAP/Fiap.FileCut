using Fiap.FileCut.Core.Enums;

namespace Fiap.FileCut.Core.Attributes;
public class MessageQueueAttribute : Attribute
{
    public Queues Queue { get; protected set; }

    public MessageQueueAttribute(Queues value)
    {
        Queue = value;
    }
}