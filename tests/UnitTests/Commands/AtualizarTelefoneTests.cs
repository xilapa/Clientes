using Clientes.Application.Clientes.Commands.AtualizarTelefone;
using Clientes.Application.Common.Resultados;
using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.Enums;
using Clientes.Domain.Clientes.Erros;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using UnitTests.Utils;


namespace UnitTests.Commands;

public sealed class AtualizarTelefoneTests : IClassFixture<BaseTestFixture>
{
    private readonly BaseTestFixture _fixture;

    public AtualizarTelefoneTests(BaseTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ContextMock.Invocations.Clear();
    }
    
    [Fact]
    public async Task ClienteQueNaoExisteNaoEhAtualizado()
    {
        // Arrange
        var handler = new AtualizarTelefoneCommandHandler(_fixture.ContextMock.Object, _fixture.TimeProvider);
        var command = new AtualizarTelefoneCommand
        {
            ClienteId = Guid.NewGuid(),
            Telefone = new AtualizarTelefoneInput{Id = Guid.NewGuid(), Numero = "12345678", DDD = "23", Tipo = TipoTelefone.Fixo}
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
    public async Task TelefoneNaoExistenteNaoEhAtualizado()
    {
        // Arrange
        var cliente = _fixture.ContextMock.Object.Clientes.First();

        var handler = new AtualizarTelefoneCommandHandler(_fixture.ContextMock.Object, _fixture.TimeProvider);
        var command = new AtualizarTelefoneCommand
        {
            ClienteId = cliente.Id.Value,
            Telefone = new AtualizarTelefoneInput{Id = Guid.NewGuid(), Numero = "12345678", DDD = "24", Tipo = TipoTelefone.Fixo}
        };

        // Act
        var resultado = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        resultado.Should().BeEquivalentTo(new Resultado(ClienteErros.TelefoneNaoEncontrado));

        _fixture.ContextMock
            .Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
    }

    [Fact]
    public async Task TelefoneEhAtualizadoComDadosValidos()
    {
        // Arrange
        var cliente = _fixture.ContextMock.Object.Clientes.AsNoTracking()
            .Include(c => c.Telefones.First())
            .First();
        var telefone = cliente.Telefones.First();

        var handler = new AtualizarTelefoneCommandHandler(_fixture.ContextMock.Object, _fixture.TimeProvider);
        var command = new AtualizarTelefoneCommand
        {
            ClienteId = cliente.Id.Value,
            Telefone = new AtualizarTelefoneInput
            {
                Id = telefone.Id.Value, Numero = "12345678", DDD = "25", Tipo = TipoTelefone.Fixo
            }
        };

        telefone.Numero.Should().NotBe(command.Telefone.NumeroCompleto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.Should().Be(Resultado.Sucesso);

        _fixture.ContextMock
            .Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        
        var clienteComTelAtualizado = _fixture.ContextMock.Object.Clientes.AsNoTracking()
            .Include(c => c.Telefones.First(t => t.Id == telefone.Id))
            .First();
        clienteComTelAtualizado.Telefones.Should().Contain(t => t.Numero == command.Telefone.NumeroCompleto);
    }
}