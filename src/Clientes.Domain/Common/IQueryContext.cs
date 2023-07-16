using Clientes.Domain.Clientes;
using Clientes.Domain.Clientes.Entities;

namespace Clientes.Domain.Common;

public interface IQueryContext
{
    IQueryable<Cliente> Clientes { get; }
    IQueryable<Telefone> Telefones { get; }
}