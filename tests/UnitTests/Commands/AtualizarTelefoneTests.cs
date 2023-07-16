using Clientes.Application.Clientes.Commands.AtualizarTelefone;
using Clientes.Application.Common.Resultados;
using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.Enums;
using Clientes.Domain.Clientes.Erros;
using Clientes.Domain.Clientes.ValueObjects;
using FluentAssertions;
using Moq;
using UnitTests.Utils;

namespace UnitTests.Commands;

public sealed class AtualizarTelefoneTests : IClassFixture<BaseTestFixture>
{
    private readonly BaseTestFixture _fixture;
    private readonly AtualizarTelefoneCommandHandler _handler;

    public AtualizarTelefoneTests(BaseTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ContextMock.Invocations.Clear();
        _handler = new AtualizarTelefoneCommandHandler(_fixture.ContextMock.Object, _fixture.TimeProvider);
    }

    [Fact]
    public async Task ClienteQueNaoExisteNaoEhAtualizado()
    {
        // Arrange
        var command = new AtualizarTelefoneCommand
        {
            ClienteId = new ClienteId(),
            TelefoneId = new TelefoneId(),
            Telefone = new TelefoneInput{Numero = "12345678", DDD = "23", Tipo = TipoTelefone.Fixo}
        };

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().BeEquivalentTo(new Resultado(ClienteErros.ClienteNaoEncontrado));

        _fixture.ContextMock
            .Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
    }

    [Fact]
    public async Task TelefoneNaoExistenteNaoEhAtualizado()
    {
        // Arrange
        var cliente = _fixture.ContextMock.GetCliente(7);

        var command = new AtualizarTelefoneCommand
        {
            ClienteId = cliente.Id,
            TelefoneId = new TelefoneId(),
            Telefone = new TelefoneInput{Numero = "12345678", DDD = "24", Tipo = TipoTelefone.Fixo}
        };

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().BeEquivalentTo(new Resultado(ClienteErros.TelefoneNaoEncontrado));

        _fixture.ContextMock
            .Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
    }

    [Fact]
    public async Task TelefoneJaCadastradoNaoEhAtualizado()
    {
        // Arrange
        var cliente = _fixture.ContextMock.GetCliente(0);
        var outroCliente = _fixture.ContextMock.GetCliente(3);

        var telefoneExistente = outroCliente.Telefones.First();

        var command = new AtualizarTelefoneCommand
        {
            ClienteId = cliente.Id,
            TelefoneId = telefoneExistente.Id,
            Telefone = new TelefoneInput
            {
                Numero = telefoneExistente.Numero, DDD = telefoneExistente.DDD, Tipo = TipoTelefone.Fixo
            }
        };

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().BeEquivalentTo(new Resultado(ClienteErros.TelefoneJaCadastrado));

        _fixture.ContextMock
            .Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
    }

    [Fact]
    public async Task NaoEhPossivelDuplicarTelefoneDoClienteNaAtualizacao()
    {
        // Arrange
        var cliente = _fixture.ContextMock.GetCliente(0);
        var telefone = cliente.Telefones.First();

        var command = new AtualizarTelefoneCommand
        {
            ClienteId = cliente.Id,
            TelefoneId = telefone.Id,
            Telefone = new TelefoneInput
            {
                Numero = telefone.Numero, DDD = telefone.DDD, Tipo = TipoTelefone.Fixo
            }
        };

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().BeEquivalentTo(new Resultado(ClienteErros.TelefoneJaCadastrado));

        _fixture.ContextMock
            .Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
    }

    [Fact]
    public async Task TelefoneEhAtualizadoComDadosValidos()
    {
        // Arrange
        var cliente = _fixture.ContextMock.GetCliente(0);
        var telefone = cliente.Telefones.First();

        var command = new AtualizarTelefoneCommand
        {
            ClienteId = cliente.Id,
            TelefoneId = telefone.Id,
            Telefone = new TelefoneInput
            {
                Numero = "12345678", DDD = "25", Tipo = TipoTelefone.Fixo
            }
        };

        telefone.DDD.Should().NotBe(command.Telefone.DDD);
        telefone.Numero.Should().NotBe(command.Telefone.Numero);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Resultado.Sucesso);

        _fixture.ContextMock
            .Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);

        var clienteComTelAtualizado = _fixture.ContextMock.GetCliente(0);
        clienteComTelAtualizado.Telefones.Should()
            .Contain(t => t.Numero == command.Telefone.Numero && t.DDD == command.Telefone.DDD);
    }
}