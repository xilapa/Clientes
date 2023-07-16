using Clientes.Application.Common.Resultados;
using Clientes.Application.Common.Validation;
using Clientes.Domain.Clientes;
using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.Erros;
using Clientes.Domain.Clientes.ValueObjects;
using Clientes.Domain.Common;
using Mediator;

namespace Clientes.Application.Clientes.Commands.AtualizarTelefone;

public sealed class AtualizarTelefoneCommand : ICommand<Resultado>, IValidable
{
    public ClienteId ClienteId { get; set; }  = null!;
    public TelefoneId TelefoneId { get; set; } = null!;
    public TelefoneInput? Telefone { get; set; }
}

public sealed class AtualizarTelefoneCommandHandler : ICommandHandler<AtualizarTelefoneCommand, Resultado>
{
    private readonly IClientesRepository _repo;
    private readonly ITimeProvider _timeProvider;
    private readonly IUow _uow;

    public AtualizarTelefoneCommandHandler(IClientesRepository repo, ITimeProvider timeProvider, IUow uow)
    {
        _repo = repo;
        _timeProvider = timeProvider;
        _uow = uow;
    }

    public async ValueTask<Resultado> Handle(AtualizarTelefoneCommand command, CancellationToken ct)
    {
        var telefoneJaCadastrado = await _repo.TelefoneJaCadastrado(command.Telefone!.DDD, command.Telefone.Numero, ct);
        if (telefoneJaCadastrado)
            return new Resultado(ClienteErros.TelefoneJaCadastrado);

        var cliente = await _repo.Get(c => c.Id == command.ClienteId, ct);

        if (cliente is null)
            return new Resultado(ClienteErros.ClienteNaoEncontrado);

        var erro = cliente.AtualizarTelefone(command.TelefoneId, command.Telefone!, _timeProvider.Now);
        if (erro != null)
            return new Resultado(erro);

        await _uow.SaveChangesAsync(ct);
        return Resultado.Sucesso;
    }
}