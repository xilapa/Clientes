namespace Clientes.Domain.Common;

public interface ITimeProvider
{
    DateTime Now { get; }
}