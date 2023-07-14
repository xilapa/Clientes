using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.Entities;
using Clientes.Domain.Clientes.ValueObjects;
using Clientes.Domain.Common;
using Clientes.Domain.Common.Exceptions;

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
    public ICollection<Telefone> Telefones => _telefones ?? new List<Telefone>();

    public void CadastrarTelefones(CadastrarTelefoneInput[] telefones, DateTime dataAtual)
    {
        _telefones ??= new List<Telefone>();
        foreach (var t in telefones)
            _telefones.Add(new Telefone(t.DDD, t.Numero, t.Tipo, dataAtual));
    }

    public void AtualizarEmail(string email, DateTime dataAtual)
    {
        Email = email;
        UltimaAtualizacao = dataAtual;
    }

    public void AtualizarTelefone(AtualizarTelefoneInput input, DateTime dataAtual)
    {
        var telefone = _telefones!.FirstOrDefault(t => t.Id == new TelefoneId(input.TelefoneId));
        if (telefone == null)
            throw new TelefoneNaoEncontradoException();
        telefone.Atualizar(input, dataAtual);
        UltimaAtualizacao = dataAtual;
    }
}