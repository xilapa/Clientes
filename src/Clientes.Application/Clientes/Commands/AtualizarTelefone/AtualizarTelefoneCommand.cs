using Clientes.Application.Common.Interfaces;
using Clientes.Application.Common.Resultados;
using Clientes.Application.Common.Validation;
using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.Erros;
using Clientes.Domain.Clientes.ValueObjects;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Application.Clientes.Commands.AtualizarTelefone;

public sealed class AtualizarTelefoneCommand : ICommand<Resultado>, IValidable
{
    public Guid ClienteId { get; set; }
    public AtualizarTelefoneInput? Telefone { get; set; }
}

public sealed class AtualizarTelefoneCommandHandler : ICommandHandler<AtualizarTelefoneCommand, Resultado>
{
    private readonly IClientesContext _context;
    private readonly ITimeProvider _timeProvider;

    public AtualizarTelefoneCommandHandler(IClientesContext context, ITimeProvider timeProvider)
    {
        _context = context;
        _timeProvider = timeProvider;
    }

    public async ValueTask<Resultado> Handle(AtualizarTelefoneCommand command, CancellationToken ct)
    {
        var cliente = await _context.Clientes
            .Include(c => c.Telefones
                .Any(t => t.Id == new TelefoneId(command.Telefone!.Id))
            )
            .Where(c => c.Id == new ClienteId(command.ClienteId))
            .SingleOrDefaultAsync(ct);

        if (cliente is null)
            return new Resultado(ClienteErros.ClienteNaoEncontrado);
        
        var telefoneJaCadastrado = _context.Clientes.AsNoTracking()
            .Any(c => c.Telefones.Any(t => t.Numero == command.Telefone!.NumeroCompleto));
        if (telefoneJaCadastrado)
            return new Resultado(ClienteErros.TelefoneJaCadastrado);

        var erro = cliente.AtualizarTelefone(command.Telefone!, _timeProvider.Now);
        if (erro != null)
            return new Resultado(erro);

        await _context.SaveChangesAsync(ct);
        return Resultado.Sucesso;
    }
}