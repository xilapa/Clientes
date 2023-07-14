using Clientes.Domain.Clientes.Enums;

namespace Clientes.Domain.Clientes.DTOs;

public abstract class BaseTelefoneInput
{
    public string DDD { get; set; } = null!;
    public string Numero { get; set; } = null!;
    public TipoTelefone Tipo { get; set; }
    public string NumeroCompleto => $"{DDD}{Numero}";
}