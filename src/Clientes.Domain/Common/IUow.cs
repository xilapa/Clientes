namespace Clientes.Domain.Common;

public interface IUow
{
    public Task<int> SaveChangesAsync(CancellationToken ct);
}