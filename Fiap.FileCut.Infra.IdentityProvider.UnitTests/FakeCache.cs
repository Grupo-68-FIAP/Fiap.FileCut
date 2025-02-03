using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Fiap.FileCut.Infra.IdentityProvider.UnitTests;

internal class FakeCache : IMemoryCache
{
    private static readonly Dictionary<object, object?> _cache = [];

    public ICacheEntry CreateEntry(object key)
    {
        return new CacheEntry(key);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _cache.Clear();
        }
    }

    public void Remove(object key)
    {
        _cache.Remove(key);
    }

    public bool TryGetValue(object key, out object? value)
    {
        return _cache.TryGetValue(key, out value);
    }

    internal class CacheEntry(object key) : ICacheEntry
    {
        public object Key { get; private set; } = key;
        public object? Value { get; set; }
        public DateTimeOffset? AbsoluteExpiration { get; set; }
        public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
        public TimeSpan? SlidingExpiration { get; set; }
        public IList<IChangeToken> ExpirationTokens { get; private set; } = [];
        public IList<PostEvictionCallbackRegistration> PostEvictionCallbacks { get; private set; } = [];
        public CacheItemPriority Priority { get; set; }
        public long? Size { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ExpirationTokens.Clear();
                PostEvictionCallbacks.Clear();
                FakeCache._cache[Key] = Value;
            }
        }
    }
}
