using Clientes.Application.Common;
using Clientes.Application.Common.Resultados;
using Clientes.Domain.Clientes;
using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.Erros;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Application.Clientes.Queries.ConsultarClientePeloTelefone;

public sealed class ConsultarClientePeloTelefoneQuery : IQuery<Resultado<ClienteView>>
{
    public string DDD { get; set; } = null!;
    public string Telefone { get; set; } = null!;
}

public sealed class ConsultarClientePeloTelefoneQueryHandler : IQueryHandler<ConsultarClientePeloTelefoneQuery, Resultado<ClienteView>>
{
    private readonly IClientesContext _context;

    public ConsultarClientePeloTelefoneQueryHandler(IClientesContext context)
    {
        _context = context;
    }

    public async ValueTask<Resultado<ClienteView>> Handle(ConsultarClientePeloTelefoneQuery query, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(query.DDD) || string.IsNullOrEmpty(query.Telefone))
            return new Resultado<ClienteView>(ClienteErros.ClienteNaoEncontrado);

        var cliente = await _context.Clientes.AsNoTrackingWithIdentityResolution()
            .Include(c => c.Telefones)
            .Where(c => c.Telefones.Any(t => t.Numero == query.Telefone && t.DDD == query.DDD))
            .Select(c => c.ToViewModel())
            .SingleOrDefaultAsync(ct);

        return cliente is null
            ? new Resultado<ClienteView>(ClienteErros.ClienteNaoEncontrado)
            : new Resultado<ClienteView>(cliente);
    }
}