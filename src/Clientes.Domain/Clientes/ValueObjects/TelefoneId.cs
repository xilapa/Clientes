using Clientes.Domain.Common;

namespace Clientes.Domain.Clientes.ValueObjects;

public sealed record TelefoneId(Guid Value) : IBaseId<Guid>
{
    public static TelefoneId New() => new(Guid.NewGuid());
    public static TelefoneId Empty => new(Guid.Empty);
    public static TelefoneId From(Guid id) => new(id);
}