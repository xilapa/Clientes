using Clientes.Application.Clientes.Commands.ExcluirCliente;
using Clientes.Domain.Clientes;
using Clientes.Domain.Clientes.Erros;
using Clientes.Infra.Persistence;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Commands;

public sealed class ExcluirClienteTestBase : IAsyncLifetime
{
    public ExcluirClienteCommandHandler Handler = null!;
    public ClientesContext Context = null!;
    private SqliteConnection _connection = null!;

    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        await _connection.OpenAsync();

        var dbContextOptsBuilder = new DbContextOptionsBuilder<ClientesContext>();
        dbContextOptsBuilder.UseSqlite(_connection);
        Context = new ClientesContext(dbContextOptsBuilder.Options);
        await Context.Database.EnsureCreatedAsync();

        Context.Clientes.Add(new Cliente("cliente", "email", DateTime.Now));
        Context.Clientes.Add(new Cliente("cliente2", "email2", DateTime.Now));
        await Context.SaveChangesAsync();

        Handler = new ExcluirClienteCommandHandler(Context);
    }

    public async Task DisposeAsync()
    {
        await _connection.CloseAsync();
        await _connection.DisposeAsync();
        await Context.DisposeAsync();
    }
}

public sealed class ExcluirClienteTests : IClassFixture<ExcluirClienteTestBase>
{
    private readonly ExcluirClienteTestBase _fixture;

    public ExcluirClienteTests(ExcluirClienteTestBase fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ClienteEhExcluido()
    {
        // Arrange
        var cliente = await _fixture.Context.Clientes.AsNoTracking().FirstAsync(e => e.Email == "email");
        var command = new  ExcluirClienteCommand {Email = cliente.Email};

        // Act
        var resultado = await _fixture.Handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Erro.Should().BeNull();
        var clienteDepois = await _fixture.Context.Clientes.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Email == "email");
        clienteDepois.Should().BeNull();
    }

    [Fact]
    public async Task ClienteNaoEhExcluidoComEmailInvalido()
    {
        // Arrange
        var command = new  ExcluirClienteCommand {Email = "email3"};
        var clientesAntes = await _fixture.Context.Clientes.AsNoTracking().ToArrayAsync();

        // Act
        var resultado = await _fixture.Handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Erro.Should().BeEquivalentTo(ClienteErros.ClienteNaoEncontrado);
        var clientesDepois = await _fixture.Context.Clientes.AsNoTracking().ToArrayAsync();
        clientesDepois.Should().BeEquivalentTo(clientesAntes);
    }
}