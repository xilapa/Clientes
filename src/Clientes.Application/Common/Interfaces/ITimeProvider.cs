namespace Clientes.Application.Common.Interfaces;

public interface ITimeProvider
{
    DateTime Now { get; }
}