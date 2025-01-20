using Fiap.FileCut.Core.Objects;

namespace Fiap.FileCut.Core.Interfaces.Services;

public interface INotifyService
{
    Task NotifyAsync<T>(NotifyContext<T> context);
}