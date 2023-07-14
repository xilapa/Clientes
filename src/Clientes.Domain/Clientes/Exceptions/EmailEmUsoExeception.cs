namespace Clientes.Domain.Common.Exceptions;

public sealed class EmailEmUsoExeception : ConflitoException
{
    public EmailEmUsoExeception() : base("Email já está em uso.")
    { }
}