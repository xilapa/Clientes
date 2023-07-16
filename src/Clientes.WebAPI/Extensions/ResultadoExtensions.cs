using Clientes.Application.Common.Resultados;
using Clientes.Domain.Common.Erros;
using Microsoft.AspNetCore.Mvc;

namespace Clientes.WebAPI.Extensions;

public static class ResultadoExtensions
{
    public static IActionResult ToActionResult<T>(this Resultado<T> resultado) where T : class
    {
        return resultado switch
        {
            { Valor: not null } => new OkObjectResult(resultado.Valor),
            { Erro.TipoErro: TipoErro.Validacao } => new BadRequestObjectResult(resultado.Erro.Mensagens),
            { Erro.TipoErro: TipoErro.NaoEncontrado } => new NotFoundObjectResult(resultado.Erro.Mensagens),
            { Erro.TipoErro: TipoErro.Conflito } => new ConflictObjectResult(resultado.Erro.Mensagens),
            _ => throw new ArgumentOutOfRangeException(nameof(resultado.Erro), "Erro inesperado")
        };
    }
    
    public static IActionResult ToActionResult(this Resultado resultado)
    {
        return resultado switch
        {
            { Erro: null } => new NoContentResult(),
            { Erro.TipoErro: TipoErro.Validacao } => new BadRequestObjectResult(resultado.Erro.Mensagens),
            { Erro.TipoErro: TipoErro.NaoEncontrado } => new NotFoundObjectResult(resultado.Erro.Mensagens),
            { Erro.TipoErro: TipoErro.Conflito } => new ConflictObjectResult(resultado.Erro.Mensagens),
            _ => throw new ArgumentOutOfRangeException(nameof(resultado.Erro), "Erro inesperado")
        };
    }
}