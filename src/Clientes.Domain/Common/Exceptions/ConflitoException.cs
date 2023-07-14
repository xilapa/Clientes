namespace Clientes.Domain.Common.Exceptions;

public abstract class ConflitoException : Exception
{
    public ConflitoException(string message) : base(message)
    { }
}