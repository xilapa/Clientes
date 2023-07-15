using Clientes.Application.Common;
using Clientes.Application.Common.Resultados;
using Clientes.Application.Common.Validation;
using Clientes.Domain.Clientes.Erros;
using Clientes.Domain.Clientes.ValueObjects;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Application.Clientes.Commands.AtualizarEmail;

public sealed class AtualizarEmailCommand : ICommand<Resultado>, IValidable
{
    public Guid ClienteId { get; set; }
    public string Email { get; set; } = null!;
}

public sealed class AtualizarEmailCommandHandler : ICommandHandler<AtualizarEmailCommand, Resultado>
{
    private readonly IClientesContext _context;
    private readonly ITimeProvider _timeProvider;

    public AtualizarEmailCommandHandler(IClientesContext context, ITimeProvider timeProvider)
    {
        _context = context;
        _timeProvider = timeProvider;
    }

    public async ValueTask<Resultado> Handle(AtualizarEmailCommand command, CancellationToken ct)
    {
        var cliente = await _context.Clientes
            .SingleOrDefaultAsync(c => c.Id == new ClienteId(command.ClienteId), ct);

        if (cliente is null)
            return new Resultado(ClienteErros.ClienteNaoEncontrado);
        
        cliente.AtualizarEmail(command.Email, _timeProvider.Now);
        await _context.SaveChangesAsync(ct);
        return Resultado.Sucesso;
    }
}