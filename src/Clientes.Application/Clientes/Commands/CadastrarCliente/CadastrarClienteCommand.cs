using Clientes.Application.Common.Interfaces;
using Clientes.Domain.Clientes;
using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Common.Exceptions;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Application.Clientes.Commands.CadastrarCliente;

public sealed class CadastrarClienteCommand : ICommand<ClienteView>
{
    public string NomeCompleto { get; set; } = null!;
    public string Email { get; set; } = null!;
    public CadastrarTelefoneInput[] Telefones { get; set; } = null!;
}

public sealed class CadastrarClienteCommandHandler : ICommandHandler<CadastrarClienteCommand, ClienteView>
{
    private readonly IClientesContext _context;
    private readonly ITimeProvider _timeProvider;

    public CadastrarClienteCommandHandler(IClientesContext context, ITimeProvider timeProvider)
    {
        _context = context;
        _timeProvider = timeProvider;
    }

    public async ValueTask<ClienteView> Handle(CadastrarClienteCommand command, CancellationToken ct)
    {
        var telsCadastrar = command.Telefones.Select(t => t.NumeroCompleto);
        var clientesExistentes = await _context.Clientes.AsNoTracking()
            .Where(c => c.Email == command.Email || c.Telefones.Any(t => telsCadastrar.Contains(t.Numero)))
            .Select(c =>
                new 
                {
                    c.Email,
                    Telefones = c.Telefones.Select(t => t.Numero)
                })
            .ToArrayAsync(ct);

        if (clientesExistentes.Length != 0 && clientesExistentes.Any(c => c.Email == command.Email))
            throw new EmailEmUsoExeception();

        if (clientesExistentes.Length != 0)
        {
            var telsCadastrados = clientesExistentes
                .SelectMany(c => c.Telefones.Where(t => telsCadastrar.Contains(t)))
                .ToArray();
            throw new TelefonesEmUsoException(telsCadastrados);
        }

        var cliente = new Cliente(command.NomeCompleto, command.Email, _timeProvider.Now);
        cliente.CadastrarTelefones(command.Telefones, _timeProvider.Now);
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync(ct);
        return cliente.ToViewModel();
    }
}