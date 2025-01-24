namespace Fiap.FileCut.Core.Interfaces.Handler;

public interface IMessageHandler<in T>
{
    Task HandleAsync(T message);
}
