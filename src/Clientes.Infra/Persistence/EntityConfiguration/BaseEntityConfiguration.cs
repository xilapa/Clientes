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
        builder.HasKey(m => m.Id);

        builder.Property(m => m.CriadoEm)
            .IsRequired();

        builder.Property(m => m.UltimaAtualizacao)
            .IsRequired();
    }
}