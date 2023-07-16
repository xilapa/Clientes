using Clientes.Application.Common.Cache;
using Clientes.Domain.Clientes.Events;
using Mediator;

namespace Clientes.Application.Clientes.EventHandlers.ClienteAlterado;

public sealed class ClienteAlteradoEventHandler : INotificationHandler<ClienteAlteradoEvent>
{
    private readonly ICache _cache;

    public ClienteAlteradoEventHandler(ICache cache)
    {
        _cache = cache;
    }

    public ValueTask Handle(ClienteAlteradoEvent notification, CancellationToken cancellationToken)
    {
        // apaga o cache da consulta de todos os clientes
        _cache.RemoveContaining(CacheKeys.ConsultarClientesQueryPrefix);

        // apaga os caches individuais relacionados ao cliente alterado
        var keys = notification.Telefones
            .Select(t => CacheKeys.ConsultarClientePeloTelefoneQuery(t.DDD, t.Numero));
        foreach (var key in keys)
            _cache.Remove(key);

        return ValueTask.CompletedTask;
    }
}