﻿using Clientes.Application.Clientes.Commands.AtualizarTelefone;
using Clientes.Domain.Clientes.ValueObjects;
using FluentAssertions;
using static Clientes.Application.Common.Constants.ApplicationErrors;

namespace UnitTests.Validators;

public sealed class AtualizarTelefoneCommandValidatorTests
{
    [Fact]
    public void DadosInvalidosRetornamErro()
    {
        // Arrange
        var command = new AtualizarTelefoneCommand
        {
            ClienteId = new ClienteId(Guid.Empty),
            TelefoneId = new TelefoneId(Guid.Empty),
            Telefone = null
        };
        var validator = new AtualizarTelefoneCommandValidator();

        // Act
        var resultado = validator.Validate(command);

        // Assert
        resultado.Errors.Select(_ => _.ErrorMessage).Should()
            .BeEquivalentTo(new []
            {
                PropriedadeVazia("ClienteId"), PropriedadeVazia("TelefoneId"),
                PropriedadeVazia("Telefone")
            }, o => o.WithoutStrictOrdering());
    }
}