using Clientes.Application.Clientes.Commands.CadastrarCliente;
using FluentAssertions;
using static Clientes.Application.Common.Constants.ApplicationErrors;

namespace UnitTests.Validators;

public sealed class CadastrarClienteCommandValidatorTests
{
    [Fact]
    public void DadosInvalidosRetornamErro()
    {
        // Arrange
        var command = new CadastrarClienteCommand
        {
            NomeCompleto = "", Email = "", Telefones = null!
        };
        var validator = new CadastrarClienteCommandValidator();

        // Act
        var resultado = validator.Validate(command);

        // Assert
        resultado.Errors.Select(_ => _.ErrorMessage).Should()
            .BeEquivalentTo(new []
            {
                PropriedadeVazia("NomeCompleto"),
                PropriedadeVazia("Email"),
                PropriedadeVazia("Telefones"),
            }, o => o.WithoutStrictOrdering());
    }
}