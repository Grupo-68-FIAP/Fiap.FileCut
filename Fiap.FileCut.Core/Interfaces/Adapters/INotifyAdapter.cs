using Fiap.FileCut.Core.Objects;

namespace Fiap.FileCut.Core.Interfaces.Adapters;

public interface INotifyAdapter
{
    void Notify<T>(NotifyContext<T> notifyContext);
}