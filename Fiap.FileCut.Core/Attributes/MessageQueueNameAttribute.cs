namespace Fiap.FileCut.Core.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class MessageQueueNameAttribute : Attribute
{
    public string QueueName { get; protected set; }

    public MessageQueueNameAttribute(string value)
    {
        QueueName = value;
    }
}