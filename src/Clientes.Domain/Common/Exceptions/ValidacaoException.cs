namespace Clientes.Domain.Common.Exceptions;

public sealed class ValidacaoException : Exception
{
    public ValidacaoException(string[] errors) : base(string.Join("; ", errors))
    { }
}