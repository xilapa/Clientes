using Clientes.Application.Clientes.Commands.ExcluirCliente;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using UnitTests.Utils;

namespace UnitTests.Commands;

public sealed class ExcluirClienteTests : IClassFixture<BaseTestFixture>
{
    private readonly BaseTestFixture _fixture;

    public ExcluirClienteTests(BaseTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void ClienteEhExcluido()
    {
        // Arrange
        var cliente = _fixture.ContextMock.Object.Clientes.AsNoTracking().First();
        var handler = new ExcluirClienteCommandHandler(_fixture.ContextMock.Object);
        var command = new ExcluirClienteCommand { Email = cliente.Email };
        
        // Act
        // var res = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        // res.Value.Should().BeOfType<ClienteExcluido>();

        // var clienteDoBanco = _fixture.ContextMock.Object.Clientes.FirstOrDefault(c => c.Email == cliente.Email);
        // clienteDoBanco.Should().BeNull();
    }
    
    // TODO: usar service collection com banco em memória
}