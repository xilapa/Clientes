using System.Reflection;
using Clientes.Domain.Clientes;
using Clientes.Domain.Clientes.Entities;
using Clientes.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Clientes.Infra.Persistence;

public sealed class ClientesContext : DbContext, IClientesContext, IQueryContext, IUow
{
    public ClientesContext(DbContextOptions<ClientesContext> opts) : base(opts)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public DbSet<Cliente> Clientes { get; set; } = null!;
    IQueryable<Cliente> IQueryContext.Clientes => Clientes.AsQueryable().AsNoTrackingWithIdentityResolution();
    public DbSet<Telefone> Telefones { get; set; } = null!;
    IQueryable<Telefone> IQueryContext.Telefones => Telefones.AsQueryable().AsNoTrackingWithIdentityResolution();
}

public interface IClientesContext
{
    public DbSet<Cliente> Clientes { get; }
    public DbSet<Telefone> Telefones { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
    ChangeTracker ChangeTracker { get; }
}