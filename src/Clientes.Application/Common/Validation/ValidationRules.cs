using System.Text.RegularExpressions;
using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.Enums;
using FluentValidation;
using static Clientes.Application.Common.Constants.ApplicationErrors;

namespace Clientes.Application.Common.Validation;

public static class ValidationRules
{
    private static Regex _validacaoDDD = new (@"^(1[1-9]|[2-9][0-9])$", RegexOptions.Compiled);
    private static Regex _validacaoTelefoneCelular = new (@"^9\d{8}$", RegexOptions.Compiled);
    private static Regex _validacaoTelefoneFixo = new (@"^\d{8}$", RegexOptions.Compiled);

    private const string Email = nameof(Email);
    private const string DDD = nameof(DDD);
    private const string Numero = nameof(Numero);

    public static void ValidarEmail<T>(this IRuleBuilderInitial<T, string> ruleBuilder)
    {
        ruleBuilder
        .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(PropriedadeVazia(Email))
            .EmailAddress()
            .WithMessage(PropriedadeComValorInvalido(Email));
    }
    
    public static void ValidarGuid<T>(this IRuleBuilderInitial<T, Guid> ruleBuilder, string propriedade)
    {
        ruleBuilder
        .Cascade(CascadeMode.Stop)
            .NotEqual(Guid.Empty)
            .WithMessage(PropriedadeVazia(propriedade));
    }
    
    public static void ValidarDDD<T>(this IRuleBuilderInitial<T, string> ruleBuilder)
    {
        ruleBuilder
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(PropriedadeVazia(DDD))
            .Must(ddd => _validacaoDDD.IsMatch(ddd))
            .WithMessage(PropriedadeComValorInvalido(DDD));
    } 

    public static void ValidarTipoTelefone<T>(this IRuleBuilderInitial<T, TipoTelefone> ruleBuilder)
    {
        ruleBuilder
            .Cascade(CascadeMode.Stop)
            .NotEqual(default(TipoTelefone))
            .WithMessage(PropriedadeComValorInvalido(nameof(BaseTelefoneInput.Tipo)))
            .Must(t => Enum.IsDefined(typeof(TipoTelefone), (int) t))
            .WithMessage(PropriedadeComValorInvalido(nameof(BaseTelefoneInput.Tipo)));
    }
    
    public static IRuleBuilderOptions<T, string> ValidarNumeroCelular<T>(this IRuleBuilderInitial<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage(PropriedadeVazia(Numero))
            .Must(ddd => _validacaoTelefoneCelular.IsMatch(ddd))
            .WithMessage((_, valor) => PropriedadeComValorInvalido(Numero, valor));
    }
    
    public static IRuleBuilderOptions<T, string> ValidarNumeroFixo<T>(this IRuleBuilderInitial<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage(PropriedadeVazia(Numero))
            .Must(ddd => _validacaoTelefoneFixo.IsMatch(ddd))
            .WithMessage((_, valor) => PropriedadeComValorInvalido(Numero, valor));
    }
}