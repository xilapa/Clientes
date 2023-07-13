using Clientes.Domain.Clientes.Enums;
using Clientes.Domain.Clientes.ValueObjects;

namespace Clientes.Domain.Clientes.DTOs;

public sealed class AtualizarTelefoneInput
{
    public TelefoneId Id { get; set; } = null!;
    public string DDD { get; set; } = null!;
    public string Numero { get; set; } = null!;
    public TipoTelefone Tipo { get; set; }
}