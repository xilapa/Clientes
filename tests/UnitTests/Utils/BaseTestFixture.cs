using Clientes.Application.Common;
using Clientes.Domain.Clientes;
using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.Enums;
using MockQueryable.Moq;
using Moq;

namespace UnitTests.Utils;

public sealed class BaseTestFixture
{
    public readonly Mock<IClientesContext> ContextMock;
    public readonly ITimeProvider TimeProvider;

    public BaseTestFixture()
    {
        var clientes = new List<Cliente>();
        var dataAtual = DateTime.Now;
        foreach (var i in Enumerable.Range(0, 9))
        {
            var cliente = new Cliente($"cliente {i}", $"email{i}@email.com", dataAtual.AddDays(i));
            var telInput = new CadastrarTelefoneInput[]
            {
                new (){DDD = "29", Numero = $"91234567{i}", Tipo = TipoTelefone.Celular},
                new (){DDD = "29", Numero = $"1234567{i}", Tipo = TipoTelefone.Fixo}
            };

            cliente.CadastrarTelefones(telInput, dataAtual.AddDays(i));
            clientes.Add(cliente);
        }
        
        var clientesMock = clientes.AsQueryable().BuildMockDbSet();
        ContextMock = new Mock<IClientesContext>();
        ContextMock.Setup(c => c.Clientes).Returns(clientesMock.Object);

        TimeProvider = new Mock<ITimeProvider>().Object;
    }
}