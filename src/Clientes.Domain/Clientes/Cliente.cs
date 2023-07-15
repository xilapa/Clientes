using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.Entities;
using Clientes.Domain.Clientes.Erros;
using Clientes.Domain.Clientes.ValueObjects;
using Clientes.Domain.Common;
using Clientes.Domain.Common.Erros;

namespace Clientes.Domain.Clientes;

public sealed class Cliente : BaseAggregateRoot<ClienteId>
{
    public Cliente(string nome, string email, DateTime dataAtual) : base(new ClienteId(), dataAtual)
    {
        NomeCompleto = nome;
        Email = email;
        _telefones = new List<Telefone>();
    }

    public string NomeCompleto { get; private set; }
    public string Email { get; private set; }

    private readonly List<Telefone> _telefones;
    public IReadOnlyCollection<Telefone> Telefones => _telefones;

    public void CadastrarTelefones(HashSet<TelefoneInput> telefones, DateTime dataAtual)
    {
        foreach (var t in telefones)
            _telefones.Add(new Telefone(t, dataAtual));
    }

    public void AtualizarEmail(string email, DateTime dataAtual)
    {
        Email = email;
        UltimaAtualizacao = dataAtual;
    }

    public Erro? AtualizarTelefone(TelefoneId id, TelefoneInput input, DateTime dataAtual)
    {
        var telefone = _telefones.Find(t => t.Id == id);
        if (telefone == null)
            return ClienteErros.TelefoneNaoEncontrado;
        telefone.Atualizar(input, dataAtual);
        UltimaAtualizacao = dataAtual;
        return null;
    }
}