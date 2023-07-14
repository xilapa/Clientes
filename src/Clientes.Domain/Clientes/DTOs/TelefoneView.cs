using Clientes.Domain.Clientes.Enums;

namespace Clientes.Domain.Clientes.DTOs;

public sealed class TelefoneView
{
    public Guid Id { get; set; }
    public string Numero { get; set; } = null!;
    public TipoTelefone Tipo { get; set; }
}