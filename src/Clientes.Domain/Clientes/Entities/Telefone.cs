using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.Enums;
using Clientes.Domain.Clientes.ValueObjects;
using Clientes.Domain.Common;

namespace Clientes.Domain.Clientes.Entities;

public sealed class Telefone : BaseEntity<TelefoneId>
{
    public Telefone(string ddd, string numero, TipoTelefone tipo, DateTime dataAtual) :
        base(TelefoneId.New(), dataAtual)
    {
        Numero = $"{ddd}{numero}";
        Tipo = tipo;
    }

    public string Numero { get; private set; }
    public TipoTelefone Tipo { get; private set; }

    public void Atualizar(AtualizarTelefoneInput input, DateTime dataAtual)
    {
        Numero = $"{input.DDD}{input.Numero}";
        Tipo = input.Tipo;
        UltimaAtualizacao = dataAtual;
    }
}