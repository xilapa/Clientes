using Clientes.Domain.Clientes.Enums;
using Clientes.Domain.Common;

namespace Clientes.Domain.Clientes.DTOs;

public sealed class TelefoneInput : BaseValueObject
{
    public string DDD { get; set; } = null!;
    public string Numero { get; set; } = null!;
    public TipoTelefone Tipo { get; set; }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return DDD;
        yield return Numero;
    }
}