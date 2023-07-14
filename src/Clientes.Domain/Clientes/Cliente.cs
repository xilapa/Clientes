using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.Entities;
using Clientes.Domain.Clientes.ValueObjects;
using Clientes.Domain.Common;

namespace Clientes.Domain.Clientes;

public sealed class Cliente : BaseAggregateRoot<ClienteId>
{
    public Cliente(string nome, string email, DateTime dataAtual) : base(ClienteId.New(), dataAtual)
    {
        NomeCompleto = nome;
        Email = email;
    }

    public string NomeCompleto { get; private set; }
    public string Email { get; private set; }

    private List<Telefone>? _telefones;
    public IEnumerable<Telefone> Telefones => _telefones ?? Enumerable.Empty<Telefone>();

    public void CadastrarTelefones(CadastrarTelefoneInput[] telefones, DateTime dataAtual)
    {
        _telefones ??= new List<Telefone>();
        foreach (var t in telefones)
            _telefones.Add(new Telefone(t.DDD, t.Numero, t.Tipo, dataAtual));
    }

    public void AtualizarEmail(string email)
    {
        Email = email;
    }

    public void AtualizarTelefone(AtualizarTelefoneInput input)
    {
        var telefone = _telefones?.First(t => t.Id == input.Id);
        telefone?.Atualizar(input);
    }
}