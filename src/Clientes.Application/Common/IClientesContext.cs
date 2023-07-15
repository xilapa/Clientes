using Clientes.Domain.Clientes;
using Clientes.Domain.Clientes.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Application.Common;

public interface IClientesContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    DbSet<Cliente> Clientes { get; }
    DbSet<Telefone> Telefones { get; }
}