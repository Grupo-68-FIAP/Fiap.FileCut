using Fiap.FileCut.Core.Objects;

namespace Fiap.FileCut.Core.Interfaces.Adapters;

public interface INotifyAdapter
{
    Task NotifyAsync<T>(NotifyContext<T> notifyContext);
}