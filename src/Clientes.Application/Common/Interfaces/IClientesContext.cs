using Clientes.Domain.Clientes;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Application.Common.Interfaces;

public interface IClientesContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    DbSet<Cliente> Clientes { get; }
}