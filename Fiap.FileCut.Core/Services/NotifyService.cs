using Fiap.FileCut.Core.Interfaces.Adapters;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;

namespace Fiap.FileCut.Core.Services;

public class NotifyService : INotifyService
{
    private readonly List<INotifyAdapter> _notifyAdapter;
    public NotifyService(
        List<INotifyAdapter> notifyAdapters
    )
    {
        _notifyAdapter = notifyAdapters;
    }
    public void Notify<T>(NotifyContext<T> context)
    {
        _notifyAdapter.ForEach(x => x.Notify(context));
    }
}
