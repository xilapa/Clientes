using Clientes.Application.Common;
using Clientes.Application.Common.Resultados;
using Clientes.Application.Common.Validation;
using Clientes.Domain.Clientes.Erros;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Application.Clientes.Commands.ExcluirCliente;

public sealed class ExcluirClienteCommand : ICommand<Resultado>, IValidable
{
    public string Email { get; set; } = null!;
}

public sealed class ExcluirClienteCommandHandler : ICommandHandler<ExcluirClienteCommand, Resultado>
{
    private readonly IClientesContext _context;

    public ExcluirClienteCommandHandler(IClientesContext context)
    {
        _context = context;
    }

    public async ValueTask<Resultado> Handle(ExcluirClienteCommand command, CancellationToken ct)
    {
        var linhasAfetadas = await _context.Clientes
            .Where(c => c.Email == command.Email)
            .ExecuteDeleteAsync(ct);
        return linhasAfetadas == 0 ? new Resultado(ClienteErros.ClienteNaoEncontrado) : Resultado.Sucesso;
    }
}