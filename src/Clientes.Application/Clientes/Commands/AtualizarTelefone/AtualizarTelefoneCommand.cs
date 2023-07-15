using Clientes.Application.Common;
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
    public Guid TelefoneId { get; set; }
    public TelefoneInput? Telefone { get; set; }
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
        var telefoneJaCadastrado = await _context.TelefoneJaCadastrado(command.Telefone!.DDD, command.Telefone.Numero, ct);
        if (telefoneJaCadastrado)
            return new Resultado(ClienteErros.TelefoneJaCadastrado);

        var clienteId = new ClienteId(command.ClienteId);
        var cliente = await _context.Clientes
            .Include(c => c.Telefones)
            .Where(c => c.Id == clienteId)
            .SingleOrDefaultAsync(ct);

        if (cliente is null)
            return new Resultado(ClienteErros.ClienteNaoEncontrado);

        var telefoneId = new TelefoneId(command.TelefoneId);
        var erro = cliente.AtualizarTelefone(telefoneId, command.Telefone!, _timeProvider.Now);
        if (erro != null)
            return new Resultado(erro);

        await _context.SaveChangesAsync(ct);
        return Resultado.Sucesso;
    }
}