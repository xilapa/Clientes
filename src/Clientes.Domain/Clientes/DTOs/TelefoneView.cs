using Clientes.Domain.Clientes.Enums;

namespace Clientes.Domain.Clientes.DTOs;

public sealed class TelefoneView
{
    public Guid Id { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime UltimaAtualizacao { get; set; }
    public string DDD { get; set; } = null!;
    public string Numero { get; set; } = null!;
    public TipoTelefone Tipo { get; set; }
}