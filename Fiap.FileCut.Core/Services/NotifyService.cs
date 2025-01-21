using Fiap.FileCut.Core.Interfaces.Adapters;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;

namespace Fiap.FileCut.Core.Services;

/// <inheritdoc/>
public class NotifyService(IEnumerable<INotifyAdapter> notifyAdapters) : INotifyService
{
    private readonly IEnumerable<INotifyAdapter> _notifyAdapter = notifyAdapters;

    /// <inheritdoc/>
    public void Notify<T>(NotifyContext<T> context)
    {
        foreach (var adapter in _notifyAdapter)
        {
            Task.Run(() => adapter.NotifyAsync(context));
        }
    }

    /// <inheritdoc/>
    public async Task NotifyAsync<T>(NotifyContext<T> context)
    {
        foreach (var adapter in _notifyAdapter)
        {
            await adapter.NotifyAsync(context);
        }
    }
}
