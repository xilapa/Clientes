using Clientes.Application.Common.Resultados;
using Clientes.Application.Common.Validation;
using Clientes.Domain.Clientes;
using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.Erros;
using Clientes.Domain.Common;
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
    private readonly IClientesRepository _repo;
    private readonly ITimeProvider _timeProvider;
    private readonly IUow _uow;

    public CadastrarClienteCommandHandler(IClientesRepository repo, ITimeProvider timeProvider, IUow uow)
    {
        _repo = repo;
        _timeProvider = timeProvider;
        _uow = uow;
    }

    public async ValueTask<Resultado<ClienteView>> Handle(CadastrarClienteCommand command, CancellationToken ct)
    {
        var emailEmUso = await _repo.EmailJaCadastrado(command.Email, ct);
        if (emailEmUso)
            return new Resultado<ClienteView>(ClienteErros.EmailJaCadastrado);

        var telsEmUso = await _repo.TelefonesJaCadastrados(command.Telefones, ct);
        if (telsEmUso.Length != 0)
            return new Resultado<ClienteView>(ClienteErros.TelefonesJaCadastrados(telsEmUso));

        var cliente = new Cliente(command.NomeCompleto, command.Email, _timeProvider.Now);
        cliente.CadastrarTelefones(command.Telefones, _timeProvider.Now);
        _repo.Add(cliente);
        await _uow.SaveChangesAsync(ct);
        return new Resultado<ClienteView>(cliente.ToViewModel());
    }
}