using Clientes.Application.Common.Resultados;
using Clientes.Application.Common.Validation;
using Clientes.Domain.Clientes;
using Clientes.Domain.Clientes.Erros;
using Clientes.Domain.Clientes.Events;
using Clientes.Domain.Common;
using Mediator;

namespace Clientes.Application.Clientes.Commands.ExcluirCliente;

public sealed class ExcluirClienteCommand : ICommand<Resultado>, IValidable
{
    public string Email { get; set; } = null!;
}

public sealed class ExcluirClienteCommandHandler : ICommandHandler<ExcluirClienteCommand, Resultado>
{
    private readonly IClientesRepository _repo;
    private readonly IMediator _mediator;
    private readonly IUow _uow;

    public ExcluirClienteCommandHandler(IClientesRepository repo, IMediator mediator, IUow uow)
    {
        _repo = repo;
        _mediator = mediator;
        _uow = uow;
    }

    public async ValueTask<Resultado> Handle(ExcluirClienteCommand command, CancellationToken ct)
    {
        var cliente = await _repo.Get(c => c.Email == command.Email, ct);

        if (cliente is null)
            return new Resultado(ClienteErros.ClienteNaoEncontrado);

        _repo.Remove(cliente);
        await _mediator.Publish(new ClienteAlteradoEvent(cliente), ct);
        await _uow.SaveChangesAsync(ct);
        return Resultado.Sucesso;
    }
}