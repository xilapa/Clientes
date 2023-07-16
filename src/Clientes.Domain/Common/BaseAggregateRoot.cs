namespace Clientes.Domain.Common;

public abstract class BaseAggregateRoot<TId> :  BaseEntity<TId>, IBaseAggregateRoot where TId : notnull
{
    // ctor pro EF
    protected BaseAggregateRoot(){}

    protected BaseAggregateRoot(TId id, DateTime dataAtual) : base(id, dataAtual)
    { }

    private readonly List<BaseEvent> _domainEvents = new();
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents;
    protected void AddDomainEvent(BaseEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}

public interface IBaseAggregateRoot
{
    IReadOnlyCollection<BaseEvent> DomainEvents { get; }
    void ClearDomainEvents();
}