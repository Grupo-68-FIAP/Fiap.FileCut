using Fiap.FileCut.Core.Objects;

namespace Fiap.FileCut.Core.Interfaces.Services;

public interface INotifyService
{
    public void Notify<T>(NotifyContext<T> notifyContext) => 
        Task.Run(async () => await NotifyAsync(notifyContext));

    Task NotifyAsync<T>(NotifyContext<T> context);
}