using System.Text.RegularExpressions;
using Clientes.Domain.Clientes.Enums;
using Clientes.Domain.Common;
using FluentValidation;
using static Clientes.Application.Common.Constants.ApplicationErrors;

namespace Clientes.Application.Common.Validation;

public static class ValidationRules
{
    private static readonly Regex ValidacaoDdd = new ("^(1[1-9]|[2-9][0-9])$", RegexOptions.Compiled);
    private static readonly Regex ValidacaoTelefoneCelular = new (@"^9\d{8}$", RegexOptions.Compiled);
    private static readonly Regex ValidacaoTelefoneFixo = new (@"^\d{8}$", RegexOptions.Compiled);

    private const string Email = nameof(Email);
    private const string DDD = nameof(DDD);
    private const string Numero = nameof(Numero);
    private const string TipoTelefone = nameof(TipoTelefone);

    public static void ValidarEmail<T>(this IRuleBuilderInitial<T, string> ruleBuilder)
    {
        ruleBuilder
        .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(PropriedadeVazia(Email))
            .EmailAddress()
            .WithMessage(PropriedadeComValorInvalido(Email));
    }

    public static void ValidarIdGuid<T>(this IRuleBuilderInitial<T, IId<Guid>> ruleBuilder, string propriedade)
    {
        ruleBuilder
        .Cascade(CascadeMode.Stop)
            .Must(id => id != null && id.Value != default && id.Value != Guid.Empty)
            .WithMessage(PropriedadeVazia(propriedade));
    }

    public static void ValidarDDD<T>(this IRuleBuilderInitial<T, string> ruleBuilder)
    {
        ruleBuilder
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(PropriedadeVazia(DDD))
            .Must(ddd => ValidacaoDdd.IsMatch(ddd))
            .WithMessage(PropriedadeComValorInvalido(DDD));
    }

    public static void ValidarTipoTelefone<T>(this IRuleBuilderInitial<T, TipoTelefone> ruleBuilder)
    {
        ruleBuilder
            .Cascade(CascadeMode.Stop)
            .NotEqual(default(TipoTelefone))
            .WithMessage(PropriedadeComValorInvalido(TipoTelefone))
            .Must(t => Enum.IsDefined(typeof(TipoTelefone), (int) t))
            .WithMessage(PropriedadeComValorInvalido(TipoTelefone));
    }

    public static IRuleBuilderOptions<T, string> ValidarNumeroCelular<T>(this IRuleBuilderInitial<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage(PropriedadeVazia(Numero))
            .Must(tel => ValidacaoTelefoneCelular.IsMatch(tel))
            .WithMessage((_, valor) => PropriedadeComValorInvalido(Numero, valor));
    }

    public static IRuleBuilderOptions<T, string> ValidarNumeroFixo<T>(this IRuleBuilderInitial<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage(PropriedadeVazia(Numero))
            .Must(tel => ValidacaoTelefoneFixo.IsMatch(tel))
            .WithMessage((_, valor) => PropriedadeComValorInvalido(Numero, valor));
    }
}