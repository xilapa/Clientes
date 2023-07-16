using Clientes.Application.Clientes.EventHandlers.ClienteAlterado;
using Clientes.Application.Clientes.EventHandlers.ClienteCadastrado;
using Clientes.Application.Common.Cache;
using Clientes.Domain.Clientes;
using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.Enums;
using Clientes.Domain.Clientes.Events;
using Moq;

namespace UnitTests.EventHandlers;

public sealed class ClienteAlteradoEventHandlerTests
{
    private readonly Cliente _cliente;
    private readonly Mock<ICache> _cacheMock;

    public ClienteAlteradoEventHandlerTests()
    {
        _cliente = new Cliente("João", "12345678910", DateTime.Now);
        var telefones = new HashSet<TelefoneInput>
        {
            new() { Numero = "123456789", DDD = "11", Tipo = TipoTelefone.Celular },
            new() { Numero = "123456789", DDD = "12", Tipo = TipoTelefone.Celular },
            new() { Numero = "123456789", DDD = "13", Tipo = TipoTelefone.Fixo },
        };
        _cliente.CadastrarTelefones(telefones, DateTime.Now);
        _cacheMock = new Mock<ICache>();
    }

    [Fact]
    public async Task ClienteAlteradoEventHandlerDeveLimparCache()
    {
        // Arrange
        var handler = new ClienteAlteradoEventHandler(_cacheMock.Object);
        var notification = new ClienteAlteradoEvent(_cliente);

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        _cacheMock.Verify(x => x.RemoveContaining(CacheKeys.ConsultarClientesQueryPrefix), Times.Once);
        foreach (var tel in _cliente.Telefones)
        {
            _cacheMock.Verify(x =>
                x.Remove(CacheKeys.ConsultarClientePeloTelefoneQuery(tel.DDD, tel.Numero)), Times.Once);
        }
    }

    [Fact]
    public async Task ClienteCadastradoEventHandlerDeveLimparCache()
    {
        // Arrange
        var handler = new ClienteCadastradoEventHandler(_cacheMock.Object);
        var notification = new ClienteCadastradoEvent();

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        _cacheMock.Verify(x => x.RemoveContaining(CacheKeys.ConsultarClientesQueryPrefix), Times.Once);
    }
}