using Fiap.FileCut.Core.Objects.Enums;

namespace Fiap.FileCut.Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class MessageQueueAttribute : Attribute
{
    public Queues Queue { get; protected set; }

    public MessageQueueAttribute(Queues value)
    {
        Queue = value;
    }
}