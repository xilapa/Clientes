using Clientes.Application.Common;
using Clientes.Application.Common.Resultados;
using Clientes.Application.Common.Validation;
using Clientes.Domain.Clientes;
using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.Erros;
using Mediator;

namespace Clientes.Application.Clientes.Commands.CadastrarCliente;

public sealed class CadastrarClienteCommand : ICommand<Resultado<ClienteView>>, IValidable
{
    public string NomeCompleto { get; set; } = null!;
    public string Email { get; set; } = null!;
    public HashSet<TelefoneInput> Telefones { get; set; } = null!;
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

        var telsEmUso = await _context.TelefonesJaCadastrados(command.Telefones, ct);
        if (telsEmUso.Length != 0)
            return new Resultado<ClienteView>(ClienteErros.TelefonesJaCadastrados(telsEmUso));

        var cliente = new Cliente(command.NomeCompleto, command.Email, _timeProvider.Now);
        cliente.CadastrarTelefones(command.Telefones, _timeProvider.Now);
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync(ct);
        return new Resultado<ClienteView>(cliente.ToViewModel());
    }
}