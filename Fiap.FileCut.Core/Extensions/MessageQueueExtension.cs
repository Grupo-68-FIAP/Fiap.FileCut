using Fiap.FileCut.Core.Attributes;
using Fiap.FileCut.Core.Objects.QueueEvents;
using System.Reflection;

namespace Fiap.FileCut.Core.Extensions;

public static class MessageQueueExtension
{
    public static string GetQueueNameFromAttribute(this BaseMessageEvent source)
    {
        return GetQueueNameFromAttribute<BaseMessageEvent>(source);
    }

    public static string GetQueueNameFromAttribute<T>(this T source)
    {
        var queueName = default(string);

        PropertyInfo[] props = typeof(T).GetProperties();

        foreach (object attr in props)
        {
            var queueAttr = attr as MessageQueueAttribute;
            
            if (queueAttr != null)
            {
                queueName = queueAttr.Queue.GetQueueNameAttribute();
                break;
            }
        }

        ArgumentNullException.ThrowIfNullOrWhiteSpace(queueName);

        return queueName;
    }

    public static string GetQueueNameFromAttribute2<T>(this T source)
    {
        var queueName = default(string);

        var props = Attribute.GetCustomAttributes(typeof(T));

        foreach (object attr in props)
        {
            var queueAttr = attr as MessageQueueAttribute;

            if (queueAttr != null)
            {
                queueName = queueAttr.Queue.GetQueueNameAttribute();
                break;
            }
        }

        ArgumentNullException.ThrowIfNullOrWhiteSpace(queueName);

        return queueName;
    }


    public static TValue GetAttributeValue<TAttribute, TValue>(
        this Type type,
        Func<TAttribute, TValue> valueSelector)
        where TAttribute : Attribute
    {
        var att = type.GetCustomAttributes(
            typeof(TAttribute), true
        ).FirstOrDefault() as TAttribute;
        if (att != null)
        {
            return valueSelector(att);
        }
        return default(TValue);
    }

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
}
