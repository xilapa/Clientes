namespace Clientes.Domain.Common.Exceptions;

public sealed class ClienteNaoEncontradoException : NaoEncontradoException
{
    public ClienteNaoEncontradoException() : base("Cliente não encontrado")
    { }
}