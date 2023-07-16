using Clientes.Application.Clientes.Commands.AtualizarEmail;
using Clientes.Application.Clientes.Commands.AtualizarTelefone;
using Clientes.Application.Clientes.Commands.CadastrarCliente;
using Clientes.Application.Clientes.Commands.ExcluirCliente;
using Clientes.Application.Clientes.Queries.ConsultarClientePeloTelefone;
using Clientes.Application.Clientes.Queries.ConsultarClientes;
using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.ValueObjects;
using Clientes.WebAPI.Extensions;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Clientes.WebAPI.Controllers;

[ApiController]
[Route("clientes")]
public sealed class ClientesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClientesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAll([FromQuery] ConsultarClientesQuery query, CancellationToken ct) =>
        Ok(await _mediator.Send(query, ct));

    [HttpGet("telefone/{ddd:required}/{numero:required}")]
    public async Task<IActionResult> GetByDddNumero([FromRoute] string ddd, [FromRoute] string numero, CancellationToken ct)
    {
        var query = new ConsultarClientePeloTelefoneQuery { DDD = ddd, Numero = numero };
        var res = await _mediator.Send(query, ct);
        return res.ToActionResult();
    }
    
    [HttpPost("")]
    public async Task<IActionResult> Post([FromBody] CadastrarClienteCommand command, CancellationToken ct)
    {
        var res = await _mediator.Send(command, ct);
        return res.ToActionResult();
    }
    
    [HttpPatch("{clienteId:guid:required}/email")]
    public async Task<IActionResult> PatchEmail([FromRoute] Guid clienteId, [FromBody] string email, CancellationToken ct)
    {
        var command = new AtualizarEmailCommand { ClienteId = new ClienteId(clienteId), Email = email };
        var res = await _mediator.Send(command, ct);
        return res.ToActionResult();
    }

    [HttpPatch("{clienteId:guid:required}/telefone/{telefoneId:guid:required}")]
    public async Task<IActionResult> PatchTelefone([FromRoute] Guid clienteId, [FromRoute] Guid telefoneId,
        [FromBody] TelefoneInput telefone)
    {
        var command = new AtualizarTelefoneCommand
        {
            ClienteId = new ClienteId(clienteId), TelefoneId = new TelefoneId(telefoneId), Telefone = telefone
        };
        var res = await _mediator.Send(command);
        return res.ToActionResult();
    }
    
    [HttpDelete("email/{email:required}")]
    public async Task<IActionResult> DeleteByEmail([FromRoute] string email, CancellationToken ct)
    {
        var command = new ExcluirClienteCommand { Email = email };
        var res = await _mediator.Send(command, ct);
        return res.ToActionResult();
    }
}