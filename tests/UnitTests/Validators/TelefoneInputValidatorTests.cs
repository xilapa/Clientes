using Clientes.Application.Common.Validation;
using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.Enums;
using FluentAssertions;
using static Clientes.Application.Common.Constants.ApplicationErrors;

namespace UnitTests.Validators;

public sealed class TelefoneInputValidatorTests
{
    public static TheoryData<TelefoneInput, string[]> DadosInvalidos = new()
    {
        {
            new TelefoneInput { Numero = "", Tipo = 0, DDD = "" },
            // Não valida o telefone pois o tipo é inválido
            new[] { PropriedadeComValorInvalido("TipoTelefone"), PropriedadeVazia("DDD") }
        },
        {
            new TelefoneInput { Numero = "123456789", Tipo = TipoTelefone.Celular, DDD = "01" },
            new[]
            {
                PropriedadeComValorInvalido("Numero", "123456789"),
                PropriedadeComValorInvalido("DDD")
            }
        },
        { new TelefoneInput { Numero = "123456789", Tipo = TipoTelefone.Fixo, DDD = "23" }, new[]
        {
            PropriedadeComValorInvalido("Numero", "123456789")
        } }
    };

    [Theory]
    [MemberData(nameof(DadosInvalidos))]
    public void DeveRetornarErrosQuandoDadosInvalidos(TelefoneInput input, string[] erros)
    {
        // Arrange
        var validator = new TelefoneInputValidator();

        // Act
        var result = validator.Validate(input);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Select(_ => _.ErrorMessage)
            .Should()
            .BeEquivalentTo(erros, o => o.WithoutStrictOrdering());
    }
}