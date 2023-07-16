using BitFaster.Caching;
using BitFaster.Caching.Lru;
using Clientes.Application.Common.Cache;

namespace Clientes.Infra.Services;

public sealed class MemoryCache : ICache
{
    private readonly IAsyncCache<string, object> _cache;

    public MemoryCache()
    {
        _cache = new ConcurrentLruBuilder<string, object>()
            .WithAtomicGetOrAdd()
            .WithCapacity(50)
            .WithExpireAfterWrite(TimeSpan.FromMinutes(2))
            .AsAsyncCache()
            .Build();
    }

    public async Task<T> GetOrAdd<T>(string key, Func<Task<object>> valueFactory)
    {
        var value = await _cache.GetOrAddAsync(key, _ => valueFactory());
        return (T)value;
    }

    public T Get<T>(string key)
    {
        var hasValue = _cache.TryGet(key, out var value);
        if(!hasValue) return default!;
        return (T)value;
    }

    public void Remove(string key) => _cache.TryRemove(key);

    public void RemoveContaining(string key)
    {
        var keys = _cache.Keys.Where(k => k.Contains(key));
        foreach (var k in keys)
            _cache.TryRemove(k);
    }
}