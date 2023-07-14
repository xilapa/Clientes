using Clientes.Application.Common.Interfaces;
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
        foreach (var i in Enumerable.Range(0, 9))
        {
            var cliente = new Cliente($"cliente {i}", $"email{i}@email.com", DateTime.Now);
            var telInput = new CadastrarTelefoneInput[]
            {
                new (){DDD = "29", Numero = $"91234567{i}", Tipo = TipoTelefone.Celular},
                new (){DDD = "29", Numero = $"1234567{i}", Tipo = TipoTelefone.Fixo}
            };

            cliente.CadastrarTelefones(telInput, DateTime.Now);
            clientes.Add(cliente);
        }
        
        var clientesMock = clientes.AsQueryable().BuildMockDbSet();
        ContextMock = new Mock<IClientesContext>();
        ContextMock.Setup(c => c.Clientes).Returns(clientesMock.Object);

        TimeProvider = new Mock<ITimeProvider>().Object;
    }
}