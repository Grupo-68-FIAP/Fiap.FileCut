namespace Fiap.FileCut.Core.Attributes;
public class MessageQueueNameAttribute : Attribute
{
    public string QueueName { get; protected set; }

    public MessageQueueNameAttribute(string value)
    {
        QueueName = value;
    }
}