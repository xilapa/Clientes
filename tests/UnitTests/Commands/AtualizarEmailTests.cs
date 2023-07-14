using Clientes.Application.Clientes.Commands.AtualizarEmail;
using Clientes.Domain.Common.Exceptions;
using FluentAssertions;
using Mediator;
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
        var act = handler.Awaiting(h => h.Handle(command, CancellationToken.None));

        // Assert
        await act.Should().ThrowExactlyAsync<ClienteNaoEncontradoException>();
        
        _fixture.ContextMock
            .Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
    }

    [Fact]
    public async Task EmailEhAtualizado()
    {
        // Arrange
        var novoEmail = "novo@email.com";
        var cliente = _fixture.ContextMock.Object.Clientes.AsNoTracking().First();
        cliente.Email.Should().NotBe(novoEmail);
        
        var handler = new AtualizarEmailCommandHandler(_fixture.ContextMock.Object, _fixture.TimeProvider);
        var command = new AtualizarEmailCommand
        {
            ClienteId = cliente.Id.Value,
            Email = novoEmail
        };
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.Should().Be(Unit.Value);

        _fixture.ContextMock
            .Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        
        var clienteAtualizado = _fixture.ContextMock.Object.Clientes.First(c => c.Id == cliente.Id);
        clienteAtualizado.Email.Should().Be(novoEmail);
    }
}