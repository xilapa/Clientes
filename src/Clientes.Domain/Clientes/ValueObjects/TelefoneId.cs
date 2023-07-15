namespace Clientes.Domain.Clientes.ValueObjects;

public sealed record TelefoneId(Guid Value)
{
    public TelefoneId() : this(Guid.NewGuid())
    { }
}