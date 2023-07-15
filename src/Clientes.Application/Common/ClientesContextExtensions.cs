using Microsoft.EntityFrameworkCore;

namespace Clientes.Application.Common;

public static class ClientesContextExtensions
{
    public static async Task<bool> EmailJaCadastrado(this IClientesContext ctx, string email, CancellationToken ct) =>
        await ctx.Clientes.AsNoTracking().AnyAsync(c => c.Email == email, ct);

    public static async Task<bool> TelefoneJaCadastrado(this IClientesContext ctx, string ddd, string telefone,
        CancellationToken ct)
    {
        return await ctx.Clientes.AsNoTracking()
            .AnyAsync(c => c.Telefones.Any(t => t.DDD == ddd && t.Numero == telefone), ct);
    }

    public static async Task<bool> TelefonesJaCadastrados(this IClientesContext ctx, string ddd, string telefone,
        CancellationToken ct)
    {
        return await ctx.Clientes.AsNoTracking()
            .AnyAsync(c => c.Telefones.Any(t => t.DDD == ddd && t.Numero == telefone), ct);
    }
}