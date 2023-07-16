using Clientes.Domain.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clientes.Infra.Persistence.EntityConfiguration;

public abstract class BaseAggregateRootConfiguration<T, TId> : BaseEntityConfiguration<T, TId>
    where T : BaseAggregateRoot<TId>
    where TId : notnull
{
    public override void Configure(EntityTypeBuilder<T> builder)
    {
        base.Configure(builder);

        builder.Ignore(a => a.DomainEvents);
    }
}