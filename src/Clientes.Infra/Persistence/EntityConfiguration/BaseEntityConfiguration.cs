using Clientes.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clientes.Infra.Persistence.EntityConfiguration;

public abstract class BaseEntityConfiguration<T, TId> : IEntityTypeConfiguration<T>
    where T : BaseEntity<TId>
    where TId : notnull
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.CriadoEm)
            .IsRequired();

        builder.Property(e => e.UltimaAtualizacao)
            .IsRequired();
    }
}