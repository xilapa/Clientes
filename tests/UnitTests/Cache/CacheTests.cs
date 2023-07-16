using Clientes.Infra.Services;
using FluentAssertions;

namespace UnitTests.Cache;

public sealed class CacheTests
{
    private readonly MemoryCache _cache;

    public CacheTests()
    {
        _cache = new MemoryCache();
    }

    [Fact]
    public async Task RemoveAsChavesExistentes()
    {
        // Arrange
        await _cache.GetOrAdd<string>("key", () => Task.FromResult("value" as object));
        await _cache.GetOrAdd<string>("key2", () => Task.FromResult("value2" as object));

        // Act
        _cache.Remove("key");

        // Assert
        _cache.Get<string>("key").Should().BeNull();
        _cache.Get<string>("key2").Should().Be("value2");
    }

    [Fact]
    public async Task RemoveContaining()
    {
        // Arrange
        await _cache.GetOrAdd<string>("key1", () => Task.FromResult("value1" as object));
        await _cache.GetOrAdd<string>("key2", () => Task.FromResult("value2" as object));

        // Act
        _cache.RemoveContaining("key");

        // Assert
        _cache.Get<string>("key1").Should().BeNull();
        _cache.Get<string>("key2").Should().BeNull();
    }

    [Fact]
    public async Task GetOrAddAdicionaValor()
    {
        // Arrange
        await _cache.GetOrAdd<string>("key", () => Task.FromResult("value" as object));

        // Act
        var result = _cache.Get<string>("key");

        // Assert
        result.Should().Be("value");
    }

    [Fact]
    public async Task GetOrAddNaoLancaExcecaoComValorNulo()
    {
        // Arrange
        await _cache.GetOrAdd<string>("key", () => Task.FromResult(null as object)!);

        // Act
        var result = _cache.Get<string>("key");

        // Assert
        result.Should().BeNull();
    }
}