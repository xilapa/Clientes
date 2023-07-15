using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.Enums;
using Clientes.Domain.Clientes.ValueObjects;
using Clientes.Domain.Common;

namespace Clientes.Domain.Clientes.Entities;

public sealed class Telefone : BaseEntity<TelefoneId>
{
    // ctor pro EF
    public Telefone()
    { }

    public Telefone(TelefoneInput telefoneInput, DateTime dataAtual) : base(new TelefoneId(), dataAtual)
    {
        DDD = telefoneInput.DDD;
        Numero = telefoneInput.Numero;
        Tipo = telefoneInput.Tipo;
    }

    public string DDD { get; private set; }
    public string Numero { get; private set; }
    public TipoTelefone Tipo { get; private set; }

    public void Atualizar(TelefoneInput input, DateTime dataAtual)
    {
        DDD = input.DDD;
        Numero = input.Numero;
        Tipo = input.Tipo;
        UltimaAtualizacao = dataAtual;
    }
}