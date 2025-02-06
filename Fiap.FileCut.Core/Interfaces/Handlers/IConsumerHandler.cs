using Fiap.FileCut.Core.Objects;

namespace Fiap.FileCut.Core.Interfaces.Handlers;

public interface IConsumerHandler<TContext> where TContext : class
{
    Task HandleAsync(NotifyContext<TContext> context);
}