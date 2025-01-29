using Fiap.FileCut.Core.Objects;

namespace Fiap.FileCut.Core.Interfaces.Handler;

public interface IMessageHandler<TContext> where TContext : class
{
    Task HandleAsync(NotifyContext<TContext> context);
}