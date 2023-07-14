namespace Clientes.Domain.Common.Exceptions;

public sealed class TelefoneEmUsoException : ConflitoException
{
    public TelefoneEmUsoException() : base("Telefone já está em uso")
    { }
}