using Clientes.Application.Common;
using Clientes.Application.Common.Resultados;
using Clientes.Domain.Clientes;
using Clientes.Domain.Clientes.DTOs;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Application.Clientes.Queries.ConsultarClientes;

public sealed class ConsultarClientesQuery : IQuery<ListaPaginada<ClienteView>>
{
    public DateTime? UltimoCriadoEm { get; set; }
    public int Take { get; set; } = 10;
}

public sealed class ConsultarClientesQueryHandler : IQueryHandler<ConsultarClientesQuery, ListaPaginada<ClienteView>>
{
    private readonly IClientesContext _context;

    public ConsultarClientesQueryHandler(IClientesContext context)
    {
        _context = context;
    }
    
    public async ValueTask<ListaPaginada<ClienteView>> Handle(ConsultarClientesQuery query, CancellationToken ct)
    {
        // Keyset pagination https://learn.microsoft.com/en-us/ef/core/querying/pagination#keyset-pagination
        var ultimoCriadoEm = query.UltimoCriadoEm ?? new DateTime();
        if (query.Take > 500)
            query.Take = 500;

        return new ListaPaginada<ClienteView>
        {
            Total = await _context.Clientes.CountAsync(ct),
            Resultados = await _context.Clientes
                .AsNoTracking()
                .OrderBy(c => c.CriadoEm)
                .Include(c => c.Telefones)
                .Where(c => !query.UltimoCriadoEm.HasValue || c.CriadoEm > ultimoCriadoEm)
                .Take(query.Take)
                .Select(c => c.ToViewModel())
                .ToArrayAsync(ct)
        };
    }
}

