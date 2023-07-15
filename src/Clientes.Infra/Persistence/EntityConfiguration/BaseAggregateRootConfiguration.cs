using Clientes.Domain.Common;

namespace Clientes.Infra.Persistence.EntityConfiguration;

public abstract class BaseAggregateRootConfiguration<T, TId> : BaseEntityConfiguration<T, TId>
    where T : BaseAggregateRoot<TId>
    where TId : notnull
{ }