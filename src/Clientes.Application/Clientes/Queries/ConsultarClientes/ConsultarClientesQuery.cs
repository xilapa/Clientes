using Clientes.Application.Common.Cache;
using Clientes.Application.Common.Resultados;
using Clientes.Domain.Clientes;
using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Application.Clientes.Queries.ConsultarClientes;

public sealed class ConsultarClientesQuery : IQuery<ListaPaginada<ClienteView>>, ICacheable
{
    public DateTime? UltimoCriadoEm { get; set; }
    public int Take { get; set; } = 10;
    public string CacheKey => CacheKeys.ConsultarClientesQuery(UltimoCriadoEm, Take);
}

public sealed class ConsultarClientesQueryHandler : IQueryHandler<ConsultarClientesQuery, ListaPaginada<ClienteView>>
{
    private readonly IQueryContext _queryContext;

    public ConsultarClientesQueryHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async ValueTask<ListaPaginada<ClienteView>> Handle(ConsultarClientesQuery query, CancellationToken ct)
    {
        // Keyset pagination https://learn.microsoft.com/en-us/ef/core/querying/pagination#keyset-pagination
        var ultimoCriadoEm = query.UltimoCriadoEm ?? new DateTime();
        if (query.Take > 500)
            query.Take = 500;

        return new ListaPaginada<ClienteView>
        {
            Total = await _queryContext.Clientes.CountAsync(ct),
            Resultados = await _queryContext.Clientes
                .OrderBy(c => c.CriadoEm)
                .Include(c => c.Telefones)
                .Where(c => !query.UltimoCriadoEm.HasValue || c.CriadoEm > ultimoCriadoEm)
                .Take(query.Take)
                .Select(c => c.ToViewModel())
                .ToArrayAsync(ct)
        };
    }
}
