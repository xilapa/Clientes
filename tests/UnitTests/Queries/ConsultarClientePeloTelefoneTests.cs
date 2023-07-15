using Clientes.Application.Clientes.Queries.ConsultarClientePeloTelefone;
using Clientes.Application.Common.Resultados;
using Clientes.Domain.Clientes;
using Clientes.Domain.Clientes.Erros;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using UnitTests.Utils;

namespace UnitTests.Queries;

public sealed class ConsultarClientePeloTelefoneTests : IClassFixture<BaseTestFixture>
{
    private readonly BaseTestFixture _fixture;

    public ConsultarClientePeloTelefoneTests(BaseTestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task ClienteEhEncontrado()
    {
        // Arrange
        var cliente = _fixture.ContextMock.Object.Clientes
            .AsNoTracking()
            .Include(c => c.Telefones)
            .Last();
        var telefone = cliente.Telefones.Last();

        var handler = new ConsultarClientePeloTelefoneQueryHandler(_fixture.ContextMock.Object);
        var query = new ConsultarClientePeloTelefoneQuery
        {
            DDD = telefone.Numero[..2], Telefone = telefone.Numero[2..]
        };
        
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        
        // Assert
        result.Valor.Should().BeEquivalentTo(cliente.ToViewModel());
    }
    
    [Fact]
    public async Task ClienteNaoEhEncontrado()
    {
        // Arrange
        var handler = new ConsultarClientePeloTelefoneQueryHandler(_fixture.ContextMock.Object);
        var query = new ConsultarClientePeloTelefoneQuery
        {
            DDD = "11", Telefone = "33220099"
        };
        
        // Act
        var resultado = await handler.Handle(query, CancellationToken.None);
        
        // Assert
        resultado.Should().BeEquivalentTo(new Resultado(ClienteErros.ClienteNaoEncontrado));
    }
}