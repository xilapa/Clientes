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
        throw new NotImplementedException();
    }
}