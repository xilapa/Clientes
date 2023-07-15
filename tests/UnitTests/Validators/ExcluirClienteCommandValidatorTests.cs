using Clientes.Application.Clientes.Commands.ExcluirCliente;
using FluentAssertions;

namespace UnitTests.Validators;

public sealed class ExcluirClienteCommandValidatorTests
{
    [Theory]
    [InlineData("emailinvalido")]
    [InlineData("")]
    public void DadosInvalidosRetornamErro(string email)
    {
        // Arrange
        var command = new ExcluirClienteCommand { Email = email };
        var validator = new ExcluirClienteCommandValidator();

        // Act
        var resultado = validator.Validate(command);

        // Assert
        resultado.Errors.Single().ErrorMessage.Should().Contain("Email");
    }
}