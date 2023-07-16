using Clientes.Application.Clientes.Queries.ConsultarClientes;
using Clientes.Application.Common.Cache;
using Clientes.Application.Common.Resultados;
using Clientes.Domain.Clientes.DTOs;
using Clientes.Infra.Services;
using FluentAssertions;
using Mediator;
using Moq;
using UnitTests.Utils;

namespace UnitTests.Cache;

public sealed class CacheBehaviourTests : IClassFixture<BaseTestFixture>
{
    private readonly BaseTestFixture _fixture;
    private readonly MemoryCache _cache;
    private readonly CacheBehaviour<ConsultarClientesQuery, ListaPaginada<ClienteView>> _cacheBehaviour;

    public CacheBehaviourTests(BaseTestFixture fixture)
    {
        _fixture = fixture;
        _cache = new MemoryCache();
        _cacheBehaviour = new CacheBehaviour<ConsultarClientesQuery, ListaPaginada<ClienteView>>(_cache);
    }

    [Fact]
    public async Task DeveRetornarDoCache()
    {
        // Arrange
        var query = new ConsultarClientesQuery();

        // criar cache
        var handler = new ConsultarClientesQueryHandler(_fixture.QueryContextMock.Object);
        var cachedResult = await handler.Handle(query, CancellationToken.None);
        await _cache.GetOrAdd<ListaPaginada<ClienteView>>(query.CacheKey, () =>
            Task.FromResult(cachedResult as object));

        var mockedHandler = new Mock<IQueryHandler<ConsultarClientesQuery, ListaPaginada<ClienteView>>>();

        // Act
        var result = await _cacheBehaviour
            .Handle(query, CancellationToken.None, mockedHandler.Object.Handle);

        // Assert
        result.Should().BeSameAs(cachedResult);

        mockedHandler
            .Verify(x =>
                x.Handle(It.IsAny<ConsultarClientesQuery>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task QuandoNaoHaCacheChamaHandler()
    {
        // Arrange
        var query = new ConsultarClientesQuery();

        var mockedHandler = new Mock<IQueryHandler<ConsultarClientesQuery, ListaPaginada<ClienteView>>>();

        // Act
        await _cacheBehaviour
            .Handle(query, CancellationToken.None, mockedHandler.Object.Handle);

        // Assert
        mockedHandler
            .Verify(x =>
                x.Handle(It.IsAny<ConsultarClientesQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}