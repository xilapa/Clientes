using Clientes.Application.Clientes.Commands.ExcluirCliente;
using Clientes.Domain.Clientes;
using Clientes.Domain.Clientes.Erros;
using Clientes.Domain.Clientes.Events;
using Clientes.Domain.Common;
using Clientes.Infra.Persistence;
using Clientes.Infra.Persistence.Repositories;
using FluentAssertions;
using Mediator;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace UnitTests.Commands;

public sealed class ExcluirClienteTestBase : IAsyncLifetime
{
    public ExcluirClienteCommandHandler Handler = null!;
    public ClientesContext ClientesContext = null!;
    private SqliteConnection _connection = null!;
    public List<BaseEvent> DomainEvents = null!;

    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        await _connection.OpenAsync();

        var dbContextOptsBuilder = new DbContextOptionsBuilder<ClientesContext>();
        dbContextOptsBuilder.UseSqlite(_connection);
        ClientesContext = new ClientesContext(dbContextOptsBuilder.Options);
        await ClientesContext.Database.EnsureCreatedAsync();

        ClientesContext.Clientes.Add(new Cliente("cliente", "email", DateTime.Now));
        ClientesContext.Clientes.Add(new Cliente("cliente2", "email2", DateTime.Now));
        await ClientesContext.SaveChangesAsync();

        var repo = new ClientesRepository(ClientesContext);

        DomainEvents = new List<BaseEvent>();
        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Publish(It.IsAny<BaseEvent>(), It.IsAny<CancellationToken>()))
            .Callback<BaseEvent, CancellationToken>((e, _) => DomainEvents.Add(e));
        var uow = new Uow(ClientesContext, mediatorMock.Object);

        Handler = new ExcluirClienteCommandHandler(repo, mediatorMock.Object, uow);
    }

    public async Task DisposeAsync()
    {
        await _connection.CloseAsync();
        await _connection.DisposeAsync();
        await ClientesContext.DisposeAsync();
    }
}

public sealed class ExcluirClienteTests : IClassFixture<ExcluirClienteTestBase>
{
    private readonly ExcluirClienteTestBase _fixture;

    public ExcluirClienteTests(ExcluirClienteTestBase fixture)
    {
        _fixture = fixture;
        _fixture.DomainEvents.Clear();
    }

    [Fact]
    public async Task ClienteEhExcluido()
    {
        // Arrange
        var cliente = await _fixture.ClientesContext.Clientes
            .Include(c => c.Telefones)
            .AsNoTracking().FirstAsync(e => e.Email == "email");
        var command = new  ExcluirClienteCommand {Email = cliente.Email};
        var eventoEsperado = new ClienteAlteradoEvent(cliente);

        // Act
        var resultado = await _fixture.Handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Erro.Should().BeNull();
        var clienteDepois = await _fixture.ClientesContext.Clientes.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Email == "email");
        clienteDepois.Should().BeNull();

        var evento = _fixture.DomainEvents.Single(e => e is ClienteAlteradoEvent);
        evento.Should().BeEquivalentTo(eventoEsperado);
    }

    [Fact]
    public async Task ClienteNaoEhExcluidoComEmailInvalido()
    {
        // Arrange
        var command = new  ExcluirClienteCommand {Email = "email3"};
        var clientesAntes = await _fixture.ClientesContext.Clientes.AsNoTracking().ToArrayAsync();

        // Act
        var resultado = await _fixture.Handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Erro.Should().BeEquivalentTo(ClienteErros.ClienteNaoEncontrado);
        var clientesDepois = await _fixture.ClientesContext.Clientes.AsNoTracking().ToArrayAsync();
        clientesDepois.Should().BeEquivalentTo(clientesAntes);
        _fixture.DomainEvents.Should().BeEmpty();
    }
}