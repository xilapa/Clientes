namespace Clientes.Domain.Clientes.DTOs;

public sealed class ClienteView
{
    public Guid Id { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime UltimaAtualizacao { get; set; }
    public string NomeCompleto { get; set; } = null!;
    public string Email { get; set; } = null!;
    public TelefoneView[] Telefones { get; set; } = null!;
}