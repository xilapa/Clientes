using System.ComponentModel.DataAnnotations;

namespace Clientes.Infra;

public class Settings
{
    [Required]
    public string ConnectionString { get; set; } = null!;
}