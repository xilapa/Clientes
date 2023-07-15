using Clientes.Domain.Common;

namespace Clientes.Domain.Clientes.ValueObjects;

public sealed record ClienteId(Guid Value) : IId<Guid>
{
    public ClienteId() : this(Guid.NewGuid())
    { }
}