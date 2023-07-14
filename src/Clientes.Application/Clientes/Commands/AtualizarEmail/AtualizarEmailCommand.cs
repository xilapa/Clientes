using Clientes.Application.Common.Interfaces;
using Clientes.Domain.Clientes.ValueObjects;
using Clientes.Domain.Common.Exceptions;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Application.Clientes.Commands.AtualizarEmail;

public sealed class AtualizarEmailCommand : ICommand
{
    public Guid ClienteId { get; set; }
    public string Email { get; set; } = null!;
}

public sealed class AtualizarEmailCommandHandler : ICommandHandler<AtualizarEmailCommand>
{
    private readonly IClientesContext _context;
    private readonly ITimeProvider _timeProvider;

    public AtualizarEmailCommandHandler(IClientesContext context, ITimeProvider timeProvider)
    {
        _context = context;
        _timeProvider = timeProvider;
    }

    public async ValueTask<Unit> Handle(AtualizarEmailCommand command, CancellationToken ct)
    {
        var cliente = await _context.Clientes
            .SingleOrDefaultAsync(c => c.Id == new ClienteId(command.ClienteId), ct);
        
        if (cliente is null)
            throw new ClienteNaoEncontradoException();
        
        cliente.AtualizarEmail(command.Email, _timeProvider.Now);
        await _context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}