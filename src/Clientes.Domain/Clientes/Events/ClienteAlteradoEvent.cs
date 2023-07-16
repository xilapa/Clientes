using Clientes.Domain.Common;

namespace Clientes.Domain.Clientes.Events;

public sealed class ClienteAlteradoEvent : BaseEvent
{
    public ClienteAlteradoEvent(Cliente cliente)
    {
        Telefones = cliente.Telefones
            .Select(t => new TelefoneDoCliente
            {
                DDD = t.DDD, Numero = t.Numero
            })
            .ToArray();
    }

    public TelefoneDoCliente[] Telefones { get; }
}

public sealed class TelefoneDoCliente
{
    public string DDD { get; set; }
    public string Numero { get; set; }
}