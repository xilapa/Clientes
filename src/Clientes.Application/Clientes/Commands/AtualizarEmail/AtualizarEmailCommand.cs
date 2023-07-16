using Clientes.Application.Common.Resultados;
using Clientes.Application.Common.Validation;
using Clientes.Domain.Clientes;
using Clientes.Domain.Clientes.Erros;
using Clientes.Domain.Clientes.ValueObjects;
using Clientes.Domain.Common;
using Mediator;

namespace Clientes.Application.Clientes.Commands.AtualizarEmail;

public sealed class AtualizarEmailCommand : ICommand<Resultado>, IValidable
{
    public ClienteId ClienteId { get; set; } = null!;
    public string Email { get; set; } = null!;
}

public sealed class AtualizarEmailCommandHandler : ICommandHandler<AtualizarEmailCommand, Resultado>
{
    private readonly IClientesRepository _repo;
    private readonly ITimeProvider _timeProvider;
    private readonly IUow _uow;

    public AtualizarEmailCommandHandler(IClientesRepository repo, ITimeProvider timeProvider, IUow uow)
    {
        _repo = repo;
        _timeProvider = timeProvider;
        _uow = uow;
    }

    public async ValueTask<Resultado> Handle(AtualizarEmailCommand command, CancellationToken ct)
    {
        var emailEmUso = await _repo.EmailJaCadastrado(command.Email, ct);
        if (emailEmUso)
            return new Resultado(ClienteErros.EmailJaCadastrado);

        var cliente = await _repo.Get(c => c.Id == command.ClienteId, ct);

        if (cliente is null)
            return new Resultado(ClienteErros.ClienteNaoEncontrado);

        cliente.AtualizarEmail(command.Email, _timeProvider.Now);
        await _uow.SaveChangesAsync(ct);
        return Resultado.Sucesso;
    }
}