using Clientes.Application.Common.Cache;
using Clientes.Domain.Clientes.Events;
using Mediator;

namespace Clientes.Application.Clientes.EventHandlers.ClienteCadastrado;

public sealed class ClienteCadastradoEventHandler : INotificationHandler<ClienteCadastradoEvent>
{
    private readonly ICache _cache;

    public ClienteCadastradoEventHandler(ICache cache)
    {
        _cache = cache;
    }

    public ValueTask Handle(ClienteCadastradoEvent notification, CancellationToken cancellationToken)
    {
        // apaga o cache da consulta de todos os clientes
        _cache.RemoveContaining(CacheKeys.ConsultarClientesQueryPrefix);
        return ValueTask.CompletedTask;
    }
}