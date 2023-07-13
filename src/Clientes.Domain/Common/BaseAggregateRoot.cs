namespace Clientes.Domain.Common;

public abstract class BaseAggregateRoot<TId> : BaseEntity<TId> where TId : notnull
{
    protected BaseAggregateRoot(TId id, DateTime dataAtual) : base(id, dataAtual)
    { }
}