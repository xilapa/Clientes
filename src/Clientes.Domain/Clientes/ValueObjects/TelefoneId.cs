using Clientes.Domain.Common;

namespace Clientes.Domain.Clientes.ValueObjects;

public sealed record TelefoneId(Guid Value) : IId<Guid>
{
    public TelefoneId() : this(Guid.NewGuid())
    { }
}