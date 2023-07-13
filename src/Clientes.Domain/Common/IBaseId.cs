namespace Clientes.Domain.Common;

public interface IBaseId<T> where T : struct
{
    static IBaseId<T> New() => throw new NotImplementedException();
    static IBaseId<T> Empty => throw new NotImplementedException();
    static IBaseId<T> From(Guid id) => throw new NotImplementedException();
}