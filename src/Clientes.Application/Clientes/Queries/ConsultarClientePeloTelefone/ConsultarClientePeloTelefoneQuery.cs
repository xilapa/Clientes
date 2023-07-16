using Clientes.Application.Common.Cache;
using Clientes.Application.Common.Resultados;
using Clientes.Domain.Clientes;
using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.Erros;
using Clientes.Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Application.Clientes.Queries.ConsultarClientePeloTelefone;

public sealed class ConsultarClientePeloTelefoneQuery : IQuery<Resultado<ClienteView>>, ICacheable
{
    public string DDD { get; set; } = null!;
    public string Numero { get; set; } = null!;
    public string CacheKey => CacheKeys.ConsultarClientePeloTelefoneQuery(DDD, Numero);
}

public sealed class ConsultarClientePeloTelefoneQueryHandler : IQueryHandler<ConsultarClientePeloTelefoneQuery, Resultado<ClienteView>>
{
    private readonly IQueryContext _queryContext;

    public ConsultarClientePeloTelefoneQueryHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async ValueTask<Resultado<ClienteView>> Handle(ConsultarClientePeloTelefoneQuery query, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(query.DDD) || string.IsNullOrEmpty(query.Numero))
            return new Resultado<ClienteView>(ClienteErros.ClienteNaoEncontrado);

        var cliente = await _queryContext.Clientes
            .Include(c => c.Telefones)
            .Where(c => c.Telefones.Any(t => t.Numero == query.Numero && t.DDD == query.DDD))
            .Select(c => c.ToViewModel())
            .SingleOrDefaultAsync(ct);

        return cliente is null
            ? new Resultado<ClienteView>(ClienteErros.ClienteNaoEncontrado)
            : new Resultado<ClienteView>(cliente);
    }
}