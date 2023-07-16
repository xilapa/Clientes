using System.Linq.Expressions;
using Clientes.Domain.Clientes;
using Clientes.Domain.Clientes.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Infra.Persistence.Repositories;

public sealed class ClientesRepository : IClientesRepository
{
    private readonly IClientesContext _context;

    public ClientesRepository(IClientesContext context)
    {
        _context = context;
    }

    public async Task<Cliente?> Get(Expression<Func<Cliente, bool>> pred, CancellationToken ct)
    {
        return await _context.Clientes
            .Include(c => c.Telefones)
            .SingleOrDefaultAsync(pred, ct);
    }

    public void Add(Cliente cliente) => _context.Clientes.Add(cliente);

    public void Remove(Cliente cliente) => _context.Clientes.Remove(cliente);

    public async Task<bool> EmailJaCadastrado(string email, CancellationToken ct) =>
        await _context.Clientes.AsNoTracking().AnyAsync(c => c.Email == email, ct);

    public async Task<bool> TelefoneJaCadastrado(string ddd, string telefone, CancellationToken ct)
    {
        return await _context.Telefones.AsNoTracking()
            .AnyAsync(t => t.DDD == ddd && t.Numero == telefone, ct);
    }

    public async Task<string[]> TelefonesJaCadastrados(IEnumerable<TelefoneInput> telefoneInputs, CancellationToken ct)
    {
        var telsCadastrar = telefoneInputs.Select(t => t.DDD + t.Numero);
        return await _context.Telefones
            .AsNoTracking()
            .Where(t => telsCadastrar.Any(tc => tc == t.DDD + t.Numero))
            .Select(t => t.DDD + t.Numero)
            .ToArrayAsync(ct);
    }
}