using Clientes.Application.Clientes.Commands.AtualizarEmail;
using Clientes.Application.Common.Resultados;
using Clientes.Domain.Clientes.Erros;
using Clientes.Domain.Clientes.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using UnitTests.Utils;

namespace UnitTests.Commands;

public sealed class AtualizarEmailTests : IClassFixture<BaseTestFixture>
{
    private readonly BaseTestFixture _fixture;
    private readonly AtualizarEmailCommandHandler _handler;

    public AtualizarEmailTests(BaseTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.UowMock.Invocations.Clear();
        _handler = new AtualizarEmailCommandHandler(_fixture.ClientesRepository, _fixture.TimeProvider,
            _fixture.UowMock.Object);
    }

    [Fact]
    public async Task ClienteQueNaoExisteNaoEhAtualizado()
    {
        // Arrange
        var command = new AtualizarEmailCommand
        {
            ClienteId = new ClienteId(),
            Email = "email@email.com"
        };

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().BeEquivalentTo(new Resultado(ClienteErros.ClienteNaoEncontrado));

        _fixture.UowMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task NaoEhPossivelAtualizarParaEmailJaCadastrado()
    {
        // Arrange
        var cliente = _fixture.ContextMock.Object.Clientes
            .OrderBy(c => c.CriadoEm)
            .AsNoTracking()
            .First();

        var command = new AtualizarEmailCommand
        {
            ClienteId = cliente.Id,
            Email = _fixture.ContextMock.Object.Clientes.AsNoTracking().Last().Email
        };

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().BeEquivalentTo(new Resultado(ClienteErros.EmailJaCadastrado));

        _fixture.UowMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task EmailEhAtualizado()
    {
        // Arrange
        const string novoEmail = "novo@email.com";
        var cliente = _fixture.ContextMock.GetCliente(3);
        cliente.Email.Should().NotBe(novoEmail);

        var command = new AtualizarEmailCommand
        {
            ClienteId = cliente.Id,
            Email = novoEmail
        };

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().BeEquivalentTo(Resultado.Sucesso);

        _fixture.UowMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        var clienteAtualizado = _fixture.ContextMock.Object.Clientes.First(c => c.Id == cliente.Id);
        clienteAtualizado.Email.Should().Be(novoEmail);
    }
}