using System.ComponentModel.DataAnnotations;

namespace Clientes.Infra;

public sealed class Settings
{
    [Required]
    public string ConnectionString { get; set; } = null!;
}