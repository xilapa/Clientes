using Clientes.Application.Clientes.Commands.CadastrarCliente;
using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.Enums;
using Clientes.Domain.Common.Exceptions;
using FluentAssertions;
using Moq;
using UnitTests.Utils;

namespace UnitTests.Commands;

public sealed class CadastrarClienteTests : IClassFixture<BaseTestFixture>
{
    private readonly BaseTestFixture _fixture;

    public CadastrarClienteTests(BaseTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ContextMock.Invocations.Clear();
    }
    
    [Fact]
    public async Task ClienteNaoEhCadastradoComEmailExistente()
    {
        // Arrange
        var handler = new CadastrarClienteCommandHandler(_fixture.ContextMock.Object, _fixture.TimeProvider);
        var command = new CadastrarClienteCommand
        {
            NomeCompleto = "nome", Email = "email0@email.com",
            Telefones = new CadastrarTelefoneInput[]
            {
                new () {Numero = "982345678", DDD = "29", Tipo = TipoTelefone.Celular}, 
            }
        };

        // Act
        var act = handler.Awaiting(h => h.Handle(command, CancellationToken.None));
        
        // Assert
        await act.Should().ThrowAsync<EmailEmUsoExeception>();

        _fixture.ContextMock
            .Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
    }
    
    [Theory]
    [InlineData("12345678", "991234567", new []{"2912345678"})]
    [InlineData("12345678", "912345678", new []{"2912345678", "29912345678"})]
    public async Task ClienteNaoEhCadastradoComTelefoneJaExistente(string telFixo, string telCelular, string[] jaCadastrados)
    {
        // Arrange
        var handler = new CadastrarClienteCommandHandler(_fixture.ContextMock.Object, _fixture.TimeProvider);
        var command = new CadastrarClienteCommand
        {
            NomeCompleto = "nome", Email = "email@email.com",
            Telefones = new CadastrarTelefoneInput[]
            {
                new () {Numero = telFixo, DDD = "29", Tipo = TipoTelefone.Fixo}, 
                new () {Numero = telCelular, DDD = "29", Tipo = TipoTelefone.Celular}, 
            }
        };

        // Act
        var act = handler.Awaiting(h => h.Handle(command, CancellationToken.None));
        
        // Assert
        await act.Should().ThrowAsync<TelefonesEmUsoException>()
            .Where(e => jaCadastrados.All(t => e.Message.Contains(t)));


        _fixture.ContextMock
            .Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
    }

    [Fact]
    public async Task ClienteEhCadastradoComDadosValidos()
    {
        // Arrange
        var handler = new CadastrarClienteCommandHandler(_fixture.ContextMock.Object, _fixture.TimeProvider);
        var command = new CadastrarClienteCommand
        {
            NomeCompleto = "nome", Email = "email@email.com",
            Telefones = new CadastrarTelefoneInput[]
            {
                new () {Numero = "912345678", DDD = "20", Tipo = TipoTelefone.Celular},
            }
        };
        
        var expectedCliente = new ClienteView
        {
            NomeCompleto = "nome", Email = "email@email.com",
            Telefones = new TelefoneView[]
            {
                new() { Numero = "20912345678", Tipo = TipoTelefone.Celular },
            }
        };

        // Act
        var clienteView = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        clienteView!.Id.Should().NotBe(Guid.Empty);
        clienteView.Telefones.Select(t => t.Id).Should().NotBeNull();

        clienteView.Should()
            .BeEquivalentTo(expectedCliente, o =>
            {
                o.Excluding(c => c.Id);
                o.For(c => c.Telefones).Exclude(t => t.Id);
                return o;
            });

        _fixture.ContextMock
            .Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
    }
}