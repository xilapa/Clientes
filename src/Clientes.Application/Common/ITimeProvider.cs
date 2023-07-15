namespace Clientes.Application.Common;

public interface ITimeProvider
{
    DateTime Now { get; }
}