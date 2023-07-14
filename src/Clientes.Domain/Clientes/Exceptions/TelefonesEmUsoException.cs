namespace Clientes.Domain.Common.Exceptions;

public sealed class TelefonesEmUsoException : ConflitoException
{
    public TelefonesEmUsoException(string[] telefones) 
        : base($"Os telefones {string.Join(", ", telefones)} já estão em uso.")
    { }
}