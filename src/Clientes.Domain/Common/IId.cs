namespace Clientes.Domain.Common;

public interface IId<T> where T : struct
{
    T Value { get; }
}