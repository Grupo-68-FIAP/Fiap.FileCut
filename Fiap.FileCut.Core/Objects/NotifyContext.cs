namespace Fiap.FileCut.Core.Objects;

public abstract class NotifyContext<T> (T context, Guid userId)
{
    public readonly T Context = context;

    public readonly Guid UserId = userId;
}