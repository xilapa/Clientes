namespace Clientes.Domain.Clientes.DTOs;

public sealed class AtualizarTelefoneInput : BaseTelefoneInput
{
    public Guid TelefoneId { get; set; }
}