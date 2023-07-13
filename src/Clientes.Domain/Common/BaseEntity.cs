namespace Clientes.Domain.Common;

public abstract class BaseEntity<TId> where TId : notnull
{
    protected BaseEntity(TId id, DateTime dataAtual)
    {
        Id = id;
        CriadoEm = dataAtual;
        UltimaAtualizacao = dataAtual;
    }

    public TId Id { get; protected set; }
    public DateTime CriadoEm { get; protected set;}
    public DateTime UltimaAtualizacao { get; protected set; }
}