using Clientes.Application.Common;
using Clientes.Application.Common.Resultados;
using Clientes.Application.Common.Validation;
using Clientes.Domain.Clientes;
using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.Erros;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Application.Clientes.Commands.CadastrarCliente;

public sealed class CadastrarClienteCommand : ICommand<Resultado<ClienteView>>, IValidable
{
    public string NomeCompleto { get; set; } = null!;
    public string Email { get; set; } = null!;
    public TelefoneInput[] Telefones { get; set; } = null!;
}

public sealed class CadastrarClienteCommandHandler : ICommandHandler<CadastrarClienteCommand, Resultado<ClienteView>>
{
    private readonly IClientesContext _context;
    private readonly ITimeProvider _timeProvider;

    public CadastrarClienteCommandHandler(IClientesContext context, ITimeProvider timeProvider)
    {
        _context = context;
        _timeProvider = timeProvider;
    }

    public async ValueTask<Resultado<ClienteView>> Handle(CadastrarClienteCommand command, CancellationToken ct)
    {
        var emailEmUso = await _context.EmailJaCadastrado(command.Email, ct);
        if (emailEmUso)
            return new Resultado<ClienteView>(ClienteErros.EmailJaCadastrado);

        var telsCadastrar = new HashSet<TelefoneInput>(command.Telefones);

        var telsEmUso = await _context.Clientes
            .AsNoTracking()
            .Where(c => c.Telefones.Any(
                t => telsCadastrar.Any(tc => tc.DDD == t.DDD && tc.Numero == t.Numero))
            )
            .SelectMany(c => c.Telefones.Select(t => $"{t.DDD}{t.Numero}"))
            .ToArrayAsync(ct);

        if (telsEmUso.Length != 0)
            return new Resultado<ClienteView>(ClienteErros.TelefonesJaCadastrados(telsEmUso));

        var cliente = new Cliente(command.NomeCompleto, command.Email, _timeProvider.Now);
        cliente.CadastrarTelefones(telsCadastrar, _timeProvider.Now);
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync(ct);
        return new Resultado<ClienteView>(cliente.ToViewModel());
    }
}