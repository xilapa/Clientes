using Clientes.Application.Common.Interfaces;
using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.ValueObjects;
using Clientes.Domain.Common.Exceptions;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Application.Clientes.Commands.AtualizarTelefone;

public sealed class AtualizarTelefoneCommand : ICommand
{
    public Guid ClienteId { get; set; }
    public AtualizarTelefoneInput? Telefone { get; set; }
}

public sealed class AtualizarTelefoneCommandHandler : ICommandHandler<AtualizarTelefoneCommand>
{
    private readonly IClientesContext _context;
    private readonly ITimeProvider _timeProvider;

    public AtualizarTelefoneCommandHandler(IClientesContext context, ITimeProvider timeProvider)
    {
        _context = context;
        _timeProvider = timeProvider;
    }

    public async ValueTask<Unit> Handle(AtualizarTelefoneCommand command, CancellationToken ct)
    {
        var cliente = await _context.Clientes
            .Include(c => c.Telefones
                .Any(t => t.Id == new TelefoneId(command.Telefone!.TelefoneId))
            )
            .Where(c => c.Id == new ClienteId(command.ClienteId))
            .SingleOrDefaultAsync(ct);

        if (cliente is null)
            throw new ClienteNaoEncontradoException();
        
        var telefoneEmUso = _context.Clientes.AsNoTracking()
            .Any(c => c.Telefones.Any(t => t.Numero == command.Telefone!.NumeroCompleto));
        if (telefoneEmUso)
            throw new TelefoneEmUsoException();

        cliente.AtualizarTelefone(command.Telefone!, _timeProvider.Now);
        await _context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}