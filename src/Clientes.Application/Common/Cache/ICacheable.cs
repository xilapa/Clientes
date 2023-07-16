using Mediator;

namespace Clientes.Application.Common.Cache;

// interface de marcação para queries que precisam ser cacheadas
public interface ICacheable : IMessage
{
    string CacheKey { get; }
}