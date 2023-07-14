﻿using Clientes.Application.Clientes.Queries.ConsultarClientes;
using Clientes.Application.Common.Resultados;
using Clientes.Domain.Clientes;
using Clientes.Domain.Clientes.DTOs;
using FluentAssertions;
using UnitTests.Utils;

namespace UnitTests.Queries;

[UsesVerify]
public sealed class ConsultarClientesTests : IClassFixture<BaseTestFixture>
{
    private readonly BaseTestFixture _fixture;

    public ConsultarClientesTests(BaseTestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task ConsultarTodosOsClientes()
    {
        // Arrange
        var todosOsClientes = _fixture.ContextMock.Object.Clientes.OrderBy(c => c.CriadoEm).ToArray();
        
        var query = new ConsultarClientesQuery {Take = 2};
        var queryHandler = new ConsultarClientesQueryHandler(_fixture.ContextMock.Object);

        // Act && Assert
        var clientesDaPaginacao = new List<ClienteView>();
        var result = new ListaPaginada<ClienteView>();
        
        // Obtenha todos os clientes paginando de 2 em 2
        do
        {
            result = await queryHandler.Handle(query, CancellationToken.None);
            
            result.Total.Should().Be(todosOsClientes.Length);
            
            // Acumule os clientes vindos da paginação
            clientesDaPaginacao.AddRange(result.Resultados);
            
            // Atualize o ultimo criado em para a próxima paginação
            query.UltimoCriadoEm = result.Resultados.Last().CriadoEm;

        } while (result.Resultados.Length >= query.Take);

        clientesDaPaginacao.Should().BeEquivalentTo(todosOsClientes.Select(c => c.ToViewModel()));
    }

    [Fact]
    public async Task ConsultarClientesRetornaDadosCorretos()
    {
        // Arrange
        var ultimoClienteRetornado = _fixture.ContextMock.Object
            .Clientes
            .OrderBy(c => c.CriadoEm)
            .Skip(3)
            .First();

        var query = new ConsultarClientesQuery {Take = 3, UltimoCriadoEm = ultimoClienteRetornado.CriadoEm};
        var queryHandler = new ConsultarClientesQueryHandler(_fixture.ContextMock.Object);
        
        // Act
        var result = await queryHandler.Handle(query, CancellationToken.None);
        
        // Assert
        await Verify(result);
    }
}