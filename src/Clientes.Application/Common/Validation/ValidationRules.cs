using System.Text.RegularExpressions;
using FluentValidation;

namespace Clientes.Application.Common.Validation;

public static class ValidationRules
{
    private static Regex _validacaoDDD = new (@"^(1[1-9]|[2-9][0-9])$", RegexOptions.Compiled);
    private static Regex _validacaoTelefoneCelular = new (@"^9\d{8}$", RegexOptions.Compiled);
    private static Regex _validacaoTelefoneFixo = new (@"^\d{8}$", RegexOptions.Compiled);

    public static IRuleBuilderOptions<T, string> DDD<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder.Must(ddd => _validacaoDDD.IsMatch(ddd));
    
    public static IRuleBuilderOptions<T, string> TelefoneCelular<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder.Must(ddd => _validacaoTelefoneCelular.IsMatch(ddd));
    
    public static IRuleBuilderOptions<T, string> TelefoneFixo<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder.Must(ddd => _validacaoTelefoneFixo.IsMatch(ddd));
}