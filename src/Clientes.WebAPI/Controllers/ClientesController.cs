using Clientes.Application.Clientes.Commands.AtualizarEmail;
using Clientes.Application.Clientes.Commands.AtualizarTelefone;
using Clientes.Application.Clientes.Commands.CadastrarCliente;
using Clientes.Application.Clientes.Commands.ExcluirCliente;
using Clientes.Application.Clientes.Queries.ConsultarClientePeloTelefone;
using Clientes.Application.Clientes.Queries.ConsultarClientes;
using Clientes.Application.Common.Resultados;
using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.ValueObjects;
using Clientes.WebAPI.Extensions;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Clientes.WebAPI.Controllers;

[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/v1/clientes")]
[Produces("application/json")]
public sealed class ClientesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClientesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retorna todos os clientes de forma paginada.
    /// </summary>
    /// <param name="ultimaDataCriacao">Data de criação do último cliente exibido.
    /// Para obter a primeira página, informe esse valor como nulo.</param>
    /// <param name="take">Quantidade de itens por página. O maior valor possível é 500,
    /// acima disso serão retornados no máximo 500 clientes.</param>
    /// <returns>Retorna a quantidade de clientes cadastrados junto com uma lista dos clientes com
    /// seus telefones e email.</returns>
    /// <remarks>
    /// </remarks>>
    /// <response code="200">Retorna a lista de clientes cadastrados junto com a quantidade total de clientes.</response>
    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ListaPaginada<ClienteView>))]
    public async Task<IActionResult> GetAll([FromQuery] DateTime? ultimaDataCriacao, [FromQuery] int? take, CancellationToken ct)
    {
        var query = new ConsultarClientesQuery { UltimoCriadoEm = ultimaDataCriacao };
        if (take.HasValue)
            query.Take = take.Value;
        
        return Ok(await _mediator.Send(query, ct));
    }

    /// <summary>
    /// Retorna um cliente pelo seu número de telefone.
    /// </summary>
    /// <param name="ddd">Número do DDD com até dois dígitos.</param>
    /// <param name="numero">Número do celular ou telefone fixo.</param>
    /// <returns>Retorna o cliente com seu email e telefones.</returns>
    /// <response code="200">Retorna o cliente encontrado.</response>
    /// <response code="404">Quando o cliente não é informado.</response>
    [HttpGet("telefone/{ddd:required}/{numero:required}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ClienteView))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string[]))]
    public async Task<IActionResult> GetByDddNumero([FromRoute] string ddd, [FromRoute] string numero, CancellationToken ct)
    {
        var query = new ConsultarClientePeloTelefoneQuery { DDD = ddd, Numero = numero };
        var res = await _mediator.Send(query, ct);
        return res.ToActionResult();
    }
    
    /// <summary>
    /// Cadastra um cliente com seus telefones e email.
    /// </summary>
    /// <param name="command">Dados do cliente para cadastro, incluindo email e lista de telefones.</param>
    /// <returns>O cliente recém criado.</returns>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST /clientes
    ///     {
    ///         "nomeCompleto": "nome completo do cliente",
    ///         "email": "email@dominio",
    ///         "telefones": [
    ///             {
    ///                 "ddd": "DDD com até dois dígitos",
    ///                 "numero": "número do telefone fixo ou celular",
    ///                 "tipo": "fixo" // as opções são "fixo" ou "celular"
    ///             }
    ///         ]
    ///     }
    /// </remarks>
    /// <response code="200">Retorna o cliente recém criado.</response>
    /// <response code="400">Retorna uma lista de erros quando os dados informados para cadastrado são inválidos.</response>
    /// <response code="409">Retorna se o email já esta cadastrado ou a lista de telefones já cadastrados.</response>
    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ClienteView))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string[]))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(string[]))]
    public async Task<IActionResult> Post([FromBody] CadastrarClienteCommand command, CancellationToken ct)
    {
        var res = await _mediator.Send(command, ct);
        return res.ToActionResult();
    }
    
    /// <summary>
    /// Atualiza o email de um cliente.
    /// </summary>
    /// <param name="clienteId">Id do cliente.</param>
    /// <param name="email">Novo email.</param>
    /// <returns></returns>
    /// <response code="204">Retorno vazio quando o email é atualizado com sucesso.</response>
    /// <response code="400">Retorna uma lista de erros quando os dados informados são inválidos.</response>
    /// <response code="409">Retorna se o email informado já está cadastrado.</response>
    [HttpPatch("{clienteId:guid:required}/email")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string[]))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(string[]))]
    public async Task<IActionResult> PatchEmail([FromRoute] Guid clienteId, [FromBody] string email, CancellationToken ct)
    {
        var command = new AtualizarEmailCommand { ClienteId = new ClienteId(clienteId), Email = email };
        var res = await _mediator.Send(command, ct);
        return res.ToActionResult();
    }

    /// <summary>
    /// Atualiza um telefone de um cliente.
    /// </summary>
    /// <param name="clienteId">Id do cliente.</param>
    /// <param name="telefoneId">Id do telefone a ser atualizado.</param>
    /// <param name="telefone">Telefone atualizado.</param>
    /// <returns></returns>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     PATCH /clientes
    ///     {
    ///         "ddd": "DDD com até dois dígitos",
    ///         "numero": "número do telefone fixo ou celular",
    ///         "tipo": "fixo" // as opções são "fixo" ou "celular"
    ///     }
    /// </remarks>
    /// <response code="204">Retorno vazio quando o telefone é atualizado com sucesso.</response>
    /// <response code="400">Retorna uma lista de erros quando os dados informados são inválidos.</response>
    /// <response code="409">Retorna se o telefone informado já está cadastrado.</response>
    [HttpPatch("{clienteId:guid:required}/telefone/{telefoneId:guid:required}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string[]))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(string[]))]
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
    
    /// <summary>
    /// Exclui um cliente pelo seu email.
    /// </summary>
    /// <param name="email">Email do cliente a ser excluído.</param>
    /// <returns></returns>
    /// <response code="204">Retorno vazio quando o cliente é excluído com sucesso.</response>
    /// <response code="404">Quando não é encontrado cliente com o email informado.</response>
    [HttpDelete("email/{email:required}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string[]))]
    public async Task<IActionResult> DeleteByEmail([FromRoute] string email, CancellationToken ct)
    {
        var command = new ExcluirClienteCommand { Email = email };
        var res = await _mediator.Send(command, ct);
        return res.ToActionResult();
    }
}