using Clientes.Domain.Clientes.Entities;
using Clientes.Domain.Clientes.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clientes.Infra.Persistence.EntityConfiguration;

public sealed class TelefoneConfiguration : BaseEntityConfiguration<Telefone, TelefoneId>
{
    public override void Configure(EntityTypeBuilder<Telefone> builder)
    {
        base.Configure(builder);

        builder.ToTable("Telefones");

        builder.Property(t => t.Id)
            .HasConversion(id => id.Value, guid => new TelefoneId(guid));

        builder.Property(t => t.DDD)
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(t => t.Numero)
            .HasMaxLength(9)
            .IsRequired();

        builder.Property(t => t.Tipo)
            .HasMaxLength(1)
            .IsRequired();

        builder.HasIndex(nameof(Telefone.Numero), nameof(Telefone.DDD), nameof(ClienteId))
            .IsUnique();
    }
}