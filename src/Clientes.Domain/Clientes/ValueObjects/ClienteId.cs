namespace Clientes.Domain.Clientes.ValueObjects;

public sealed record ClienteId(Guid Value)
{
    public ClienteId() : this(Guid.NewGuid())
    { }
}