using Mediator;

namespace Clientes.Application.Common.Cache;

public sealed class CacheBehaviour<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse>
    where TMessage : ICacheable
{
    private readonly ICache _cache;

    public CacheBehaviour(ICache cache)
    {
        _cache = cache;
    }

    public async ValueTask<TResponse> Handle(TMessage message, CancellationToken cancellationToken,
        MessageHandlerDelegate<TMessage, TResponse> next) =>
        await _cache.GetOrAdd<TResponse>(message.CacheKey, async () => (await next(message, cancellationToken))!);
}