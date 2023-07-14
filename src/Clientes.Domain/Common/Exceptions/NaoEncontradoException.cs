namespace Clientes.Domain.Common.Exceptions;

public abstract class NaoEncontradoException : Exception
{
    protected NaoEncontradoException(string msg) : base(msg)
    { }
}