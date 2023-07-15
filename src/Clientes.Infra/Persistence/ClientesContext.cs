using System.Reflection;
using Clientes.Application.Common;
using Clientes.Domain.Clientes;
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
}