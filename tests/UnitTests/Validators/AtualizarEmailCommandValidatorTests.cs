using Clientes.Application.Clientes.Commands.AtualizarEmail;
using FluentAssertions;
using static Clientes.Application.Common.Constants.ApplicationErrors;

namespace UnitTests.Validators;

public sealed class AtualiarEmailValidatorTests
{
    public AtualiarEmailValidatorTests()
    {
        _validator = new AtualizarEmailCommandValidator();
    }

    [Fact]
    public void NaoAtualizaComEmailInvalido()
    {
        // Arrange
        var command = new AtualizarEmailCommand
        {
            ClienteId = Guid.NewGuid(),
            Email = "emailinvalido"
        };

        // Act
        var resultado = _validator.Validate(command);

        // Assert
        resultado.Errors.Single().ErrorMessage.Should()
            .BeEquivalentTo(PropriedadeComValorInvalido("Email"));
    }

    public static TheoryData<Guid> ClientesIdsInvalidos = new() { Guid.Empty, default };
    private readonly AtualizarEmailCommandValidator _validator;

    [Theory]
    [MemberData(nameof(ClientesIdsInvalidos))]
    public void NaoAtualizaComClienteIdInvalido(Guid guid)
    {
        // Arrange
        var command = new AtualizarEmailCommand
        {
            ClienteId = guid,
            Email = "email@email.com"
        };

        // Act
        var resultado =  _validator.Validate(command);

        // Assert
        resultado.Errors.Single().ErrorMessage.Should()
            .BeEquivalentTo(PropriedadeVazia("ClienteId"));
    }
}