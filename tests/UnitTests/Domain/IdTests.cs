using Clientes.Domain.Common;
using FluentAssertions;

namespace UnitTests.Domain;

public sealed class IdTests
{
    [Fact]
    public void IdsDeGuidDevemTerCtorPublico()
    {
        var ids = typeof(IId<Guid>).Assembly.GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IId<Guid>)))
            .ToArray();

        ids.Should().NotBeEmpty();

        foreach (var id in ids)
        {
            var temCtorPublico = id.GetConstructors().Any(c => c.GetParameters().Length == 0);
            temCtorPublico.Should().BeTrue();

            var instancia = Activator.CreateInstance(id) as IId<Guid>;
            instancia!.Value.Should().NotBe(Guid.Empty);
        }
    }
}