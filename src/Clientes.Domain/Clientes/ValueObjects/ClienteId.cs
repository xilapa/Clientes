using Clientes.Domain.Common;

namespace Clientes.Domain.Clientes.ValueObjects;

public sealed record ClienteId(Guid Value) : IBaseId<Guid>
{
    public static ClienteId New() => new(Guid.NewGuid());
    public static ClienteId Empty => new(Guid.Empty);
    public static ClienteId From(Guid id) => new(id);
}