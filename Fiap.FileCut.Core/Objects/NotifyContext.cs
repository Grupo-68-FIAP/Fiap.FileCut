namespace Fiap.FileCut.Core.Objects;

/// <summary>
/// Value to notify.
/// </summary>
/// <typeparam name="T">Type of value to notify.</typeparam>
/// <param name="value">Data of notification.</param>
/// <param name="userId">Id of user to notify.</param>
public class NotifyContext<T> (T value, Guid userId)
{
    /// <summary>
    /// Data of notification.
    /// </summary>
    public T Value { get; set; } = value;
    /// <summary>
    /// Id of user to notify.
    /// </summary>
    public readonly Guid UserId = userId;

    public NotifyContext<T2> Convert<T2>(T2 value)
    {
        return new NotifyContext<T2>(value, UserId);
    }
}