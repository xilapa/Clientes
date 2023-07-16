using Clientes.Domain.Clientes;
using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.Entities;
using Clientes.Domain.Clientes.Enums;
using Clientes.Domain.Common;
using Clientes.Infra.Persistence;
using Clientes.Infra.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;

namespace UnitTests.Utils;

public sealed class BaseTestFixture
{
    public readonly Mock<IClientesContext> ContextMock;
    public readonly Mock<IQueryContext> QueryContextMock;
    public readonly ITimeProvider TimeProvider;
    public readonly ClientesRepository ClientesRepository;
    public readonly Mock<IUow> UowMock;

    public BaseTestFixture()
    {
        var clientes = new List<Cliente>();
        var telefones = new List<Telefone>();
        var dataAtual = DateTime.Now;
        foreach (var i in Enumerable.Range(0, 9))
        {
            var cliente = new Cliente($"cliente {i}", $"email{i}@email.com", dataAtual.AddDays(i));
            var telInput = new HashSet<TelefoneInput>
            {
                new (){DDD = "29", Numero = $"91234567{i}", Tipo = TipoTelefone.Celular},
                new (){DDD = "29", Numero = $"1234567{i}", Tipo = TipoTelefone.Fixo}
            };

            cliente.CadastrarTelefones(telInput, dataAtual.AddDays(i));
            clientes.Add(cliente);
            telefones.AddRange(cliente.Telefones);
        }

        var clientesMock = clientes.AsQueryable().BuildMockDbSet();
        var telefonesMock = telefones.AsQueryable().BuildMockDbSet();

        ContextMock = new Mock<IClientesContext>();
        ContextMock.Setup(c => c.Clientes).Returns(clientesMock.Object);
        ContextMock.Setup(c => c.Telefones).Returns(telefonesMock.Object);

        QueryContextMock = new Mock<IQueryContext>();
        QueryContextMock.Setup(c => c.Clientes).Returns(clientesMock.Object);
        QueryContextMock.Setup(c => c.Telefones).Returns(telefonesMock.Object);

        ClientesRepository = new ClientesRepository(ContextMock.Object);

        TimeProvider = new Mock<ITimeProvider>().Object;
        UowMock = new Mock<IUow>();
    }
}

public static class ContextMockExtensions
{
    public static Cliente GetCliente(this Mock<IClientesContext> mockContext, int indice)
    {
        return mockContext.Object.Clientes
            .OrderBy(c => c.CriadoEm)
            .Include(c => c.Telefones)
            .AsNoTracking()
            .Skip(indice)
            .First();
    }
}