namespace Fiap.FileCut.Core.Objects;

public class NotifyContext<T> (T context, Guid userId)
{
    public T Context { get; set; } = context;

    public readonly Guid UserId = userId;
}