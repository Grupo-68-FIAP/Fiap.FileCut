namespace Fiap.FileCut.Core.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Delegate | AttributeTargets.Field)]
public class MessageQueueNameAttribute : Attribute
{
    public string QueueName { get; protected set; }

    public MessageQueueNameAttribute(string value)
    {
        QueueName = value;
    }
}