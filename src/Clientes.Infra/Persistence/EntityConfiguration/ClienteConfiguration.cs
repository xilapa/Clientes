using Clientes.Domain.Clientes;
using Clientes.Domain.Clientes.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clientes.Infra.Persistence.EntityConfiguration;

public sealed class ClienteConfiguration : BaseAggregateRootConfiguration<Cliente, ClienteId>
{
    public override void Configure(EntityTypeBuilder<Cliente> builder)
    {
        base.Configure(builder);

        builder.ToTable("Clientes");

        builder.Property(c => c.Id)
            .HasConversion(id => id.Value, guid => new ClienteId(guid));

        builder.Property(c => c.Email)
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(c => c.NomeCompleto)
            .HasMaxLength(128)
            .IsRequired();

        builder.HasIndex(c => c.Email)
            .IsUnique();

        builder.HasIndex(c => c.CriadoEm)
            .IsDescending(false)
            .IsUnique(false);

        builder.HasMany(c => c.Telefones)
            .WithOne()
            .IsRequired()
            .HasForeignKey(nameof(ClienteId));

        builder.Metadata
            .FindNavigation(nameof(Cliente.Telefones))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}