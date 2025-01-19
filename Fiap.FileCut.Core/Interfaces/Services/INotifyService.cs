using Fiap.FileCut.Core.Objects;

namespace Fiap.FileCut.Core.Interfaces.Services;

public interface INotifyService
{
    void Notify<T>(NotifyContext<T> context);
}