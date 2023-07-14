using Clientes.Application.Common.Interfaces;
using Clientes.Domain.Clientes;
using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Common.Exceptions;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Application.Clientes.Queries.ConsultarClientePeloTelefone;

public sealed class ConsultarClientePeloTelefoneQuery : IQuery<ClienteView>
{
    public string DDD { get; set; } = null!;
    public string Telefone { get; set; } = null!;
}

public sealed class ConsultarClientePeloTelefoneQueryHandler : IQueryHandler<ConsultarClientePeloTelefoneQuery, ClienteView>
{
    private readonly IClientesContext _context;

    public ConsultarClientePeloTelefoneQueryHandler(IClientesContext context)
    {
        _context = context;
    }
    
    public async ValueTask<ClienteView> Handle(ConsultarClientePeloTelefoneQuery query, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(query.DDD) || string.IsNullOrEmpty(query.Telefone))
            throw new ClienteNaoEncontradoException();

        return await _context.Clientes.AsNoTrackingWithIdentityResolution()
            .Include(c => c.Telefones)
            .Where(c => c.Telefones.Any(t => t.Numero == $"{query.DDD}{query.Telefone}"))
            .Select(c => c.ToViewModel())
            .SingleOrDefaultAsync(ct) ?? throw new ClienteNaoEncontradoException();
    }
}