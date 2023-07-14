using Clientes.Application.Clientes.Commands.AtualizarTelefone;
using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.Enums;
using Clientes.Domain.Common.Exceptions;
using FluentAssertions;
using Mediator;
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
            Telefone = new AtualizarTelefoneInput{TelefoneId = Guid.NewGuid(), Numero = "12345678", DDD = "23", Tipo = TipoTelefone.Fixo}
        };
        
        // Act
        var act = handler.Awaiting(h => h.Handle(command, CancellationToken.None));

        // Assert
        await act.Should().ThrowAsync<ClienteNaoEncontradoException>();

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
            Telefone = new AtualizarTelefoneInput{TelefoneId = Guid.NewGuid(), Numero = "12345678", DDD = "23", Tipo = TipoTelefone.Fixo}
        };

        // Act
        var act = handler.Awaiting(h => h.Handle(command, CancellationToken.None));
        
        // Assert
        await act.Should().ThrowAsync<TelefoneEmUsoException>();

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
                TelefoneId = telefone.Id.Value, Numero = "12345678", DDD = "23", Tipo = TipoTelefone.Fixo
            }
        };

        telefone.Numero.Should().NotBe(command.Telefone.NumeroCompleto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.Should().Be(Unit.Value);

        _fixture.ContextMock
            .Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        
        var clienteComTelAtualizado = _fixture.ContextMock.Object.Clientes.AsNoTracking()
            .Include(c => c.Telefones.First(t => t.Id == telefone.Id))
            .First();
    }
}