using Clientes.Application.Common.Interfaces;
using Clientes.Domain.Common.Exceptions;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Application.Clientes.Commands.ExcluirCliente;

public sealed class ExcluirClienteCommand : ICommand
{
    public string Email { get; set; } = null!;
}

public sealed class ExcluirClienteCommandHandler : ICommandHandler<ExcluirClienteCommand>
{
    private readonly IClientesContext _context;

    public ExcluirClienteCommandHandler(IClientesContext context)
    {
        _context = context;
    }

    public async ValueTask<Unit> Handle(ExcluirClienteCommand command, CancellationToken ct)
    {
        var linhasAfetadas = await _context.Clientes
            .Where(c => c.Email == command.Email)
            .ExecuteDeleteAsync(ct);
        return linhasAfetadas == 0 ? throw new ClienteNaoEncontradoException() : Unit.Value;
    }
}