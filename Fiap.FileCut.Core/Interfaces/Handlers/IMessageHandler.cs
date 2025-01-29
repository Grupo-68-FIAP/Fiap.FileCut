using Fiap.FileCut.Core.Objects;

namespace Fiap.FileCut.Core.Interfaces.Handlers;

public interface IMessageHandler<TContext> where TContext : class
{
    Task HandleAsync(NotifyContext<TContext> context);
}