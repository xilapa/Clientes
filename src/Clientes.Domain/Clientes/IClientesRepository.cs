using System.Linq.Expressions;
using Clientes.Domain.Clientes.DTOs;

namespace Clientes.Domain.Clientes;

public interface IClientesRepository
{
    Task<Cliente?> Get(Expression<Func<Cliente, bool>> pred, CancellationToken ct);

    void Add(Cliente cliente);

    void Remove(Cliente cliente);

    Task<bool> EmailJaCadastrado(string email, CancellationToken ct);

    Task<bool> TelefoneJaCadastrado(string ddd, string telefone, CancellationToken ct);

    Task<string[]> TelefonesJaCadastrados(IEnumerable<TelefoneInput> telefoneInputs, CancellationToken ct);
}