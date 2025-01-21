using Fiap.FileCut.Core.Objects;

namespace Fiap.FileCut.Core.Interfaces.Services;

/// <summary>
/// Generic service to notify.
/// </summary>
public interface INotifyService
{
    /// <summary>
    /// Notify all adapters.
    /// </summary>
    /// <typeparam name="T">Type of data to notify.</typeparam>
    /// <param name="context">Context to notify.</param>
    void Notify<T>(NotifyContext<T> context);
    /// <summary>
    /// Notify all adapters asynchronously.
    /// </summary>
    /// <typeparam name="T">Type of data to notify.</typeparam>
    /// <param name="context">Context to notify.</param>
    /// <returns>Asynchronous operation.</returns>
    Task NotifyAsync<T>(NotifyContext<T> context);
}