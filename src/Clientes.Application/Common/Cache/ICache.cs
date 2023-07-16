namespace Clientes.Application.Common.Cache;

public interface ICache
{
    Task<T> GetOrAdd<T>(string key, Func<Task<object>> valueFactory);
    T Get<T>(string key);
    void Remove(string key);
    void RemoveContaining(string key);
}