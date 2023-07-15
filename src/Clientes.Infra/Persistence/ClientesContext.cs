using System.Reflection;
using Clientes.Application.Common;
using Clientes.Domain.Clientes;
using Clientes.Domain.Clientes.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Infra.Persistence;

public sealed class ClientesContext : DbContext, IClientesContext
{
    public ClientesContext(DbContextOptions<ClientesContext> opts) : base(opts)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public DbSet<Cliente> Clientes { get; set; } = null!;
    public DbSet<Telefone> Telefones { get; set; } = null!;
}