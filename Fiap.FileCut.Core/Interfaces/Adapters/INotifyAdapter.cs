using Fiap.FileCut.Core.Objects;

namespace Fiap.FileCut.Core.Interfaces.Adapters;

/// <summary>
/// Adapter to notify.
/// </summary>
public interface INotifyAdapter
{
    /// <summary>
    /// Send notification.
    /// </summary>
    /// <typeparam name="T">Type of data to notify.</typeparam>
    /// <param name="notifyContext">Data context to notify.</param>
    /// <returns></returns>
    Task NotifyAsync<T>(NotifyContext<T> notifyContext);
}