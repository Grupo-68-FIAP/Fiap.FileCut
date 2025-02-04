namespace Fiap.FileCut.Core.Objects;

/// <summary>
/// Context to notify.
/// </summary>
/// <typeparam name="T">Type of context to notify.</typeparam>
/// <param name="context">Data of notification.</param>
/// <param name="userId">Id of user to notify.</param>
public class NotifyContext<T> (T context, Guid userId)
{
    /// <summary>
    /// Data of notification.
    /// </summary>
    public T Context { get; set; } = context;
    /// <summary>
    /// Id of user to notify.
    /// </summary>
    public readonly Guid UserId = userId;

    public NotifyContext<T2> Convert<T2>(T2 context)
    {
        return new NotifyContext<T2>(context, UserId);
    }
}