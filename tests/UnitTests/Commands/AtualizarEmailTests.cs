using Clientes.Application.Clientes.Commands.AtualizarEmail;
using Clientes.Application.Common.Resultados;
using Clientes.Domain.Clientes.Erros;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using UnitTests.Utils;

namespace UnitTests.Commands;

public sealed class AtualizarEmailTests : IClassFixture<BaseTestFixture>
{
    private readonly BaseTestFixture _fixture;

    public AtualizarEmailTests(BaseTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ContextMock.Invocations.Clear();
    }

    [Fact]
    public async Task ClienteQueNaoExisteNaoEhAtualizado()
    {
        // Arrange
        var handler = new AtualizarEmailCommandHandler(_fixture.ContextMock.Object, _fixture.TimeProvider);
        var command = new AtualizarEmailCommand
        {
            ClienteId = Guid.NewGuid(),
            Email = "email@email.com"
        };
        
        // Act
        var resultado = await handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().BeEquivalentTo(new Resultado(ClienteErros.ClienteNaoEncontrado));

        _fixture.ContextMock
            .Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
    }

    [Fact]
    public async Task EmailEhAtualizado()
    {
        // Arrange
        const string novoEmail = "novo@email.com";
        var cliente = _fixture.ContextMock.Object.Clientes.AsNoTracking().First();
        cliente.Email.Should().NotBe(novoEmail);
        
        var handler = new AtualizarEmailCommandHandler(_fixture.ContextMock.Object, _fixture.TimeProvider);
        var command = new AtualizarEmailCommand
        {
            ClienteId = cliente.Id.Value,
            Email = novoEmail
        };
        
        // Act
        var resultado = await handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().BeEquivalentTo(Resultado.Sucesso);

        _fixture.ContextMock
            .Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        
        var clienteAtualizado = _fixture.ContextMock.Object.Clientes.First(c => c.Id == cliente.Id);
        clienteAtualizado.Email.Should().Be(novoEmail);
    }
}