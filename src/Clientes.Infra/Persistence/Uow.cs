using Clientes.Domain.Common;
using Mediator;

namespace Clientes.Infra.Persistence;

public sealed class Uow : IUow
{
    private readonly IClientesContext _context;
    private readonly IMediator _mediator;

    public Uow(IClientesContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        await DispatchEvents(_mediator, cancellationToken);
        var res = await _context.SaveChangesAsync(cancellationToken);
        return res;
    }

    private async Task DispatchEvents(IMediator mediator, CancellationToken ct)
    {
        var agregados = _context.ChangeTracker
            .Entries<IBaseAggregateRoot>()
            .Where(a => a.Entity.DomainEvents.Count != 0)
            .Select(a => a.Entity)
            .ToArray();

        if (agregados.Length == 0)
            return;

        foreach (var agregado in agregados)
        {
            foreach (var domainEvent in agregado.DomainEvents)
                await mediator.Publish(domainEvent, ct);

            agregado.ClearDomainEvents();
        }
    }
}