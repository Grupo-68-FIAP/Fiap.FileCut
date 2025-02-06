using Fiap.FileCut.Core.Attributes;
using System.Reflection;

namespace Fiap.FileCut.Core.Extensions;

public static class MessageQueueExtension
{
    /// <summary>
    /// Returns an Enum AlternateValue attribute
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string GetQueueNameAttribute(this Enum source)
    {
        Type Type = source.GetType();

        FieldInfo? FieldInfo = Type.GetField(source.ToString());

        ArgumentNullException.ThrowIfNull(FieldInfo);

        MessageQueueNameAttribute? attr = FieldInfo.GetCustomAttribute(
            typeof(MessageQueueNameAttribute)
        ) as MessageQueueNameAttribute;

        ArgumentNullException.ThrowIfNullOrWhiteSpace(attr?.QueueName);     

        return attr.QueueName;
    }

    public static string? GetQueueName<T>(string? defaultQueue = default(string))
    {
        var queueName = defaultQueue;
        
        var props = Attribute.GetCustomAttributes(typeof(T));
        foreach (object attr in props)
        {
            if (attr is MessageQueueAttribute a)
            {
                queueName = a.Queue.GetQueueNameAttribute();
                break;
            }
        }

        return queueName;
    }
}
