namespace Clientes.Domain.Common.Exceptions;

public sealed class TelefoneNaoEncontradoException : NaoEncontradoException
{
    public TelefoneNaoEncontradoException() : base("Telefone não encontrado")
    { }
}