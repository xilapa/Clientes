using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.Entities;

namespace Clientes.Domain.Clientes;

public static class ClienteMappingExtensions
{
    public static ClienteView ToViewModel(this Cliente cliente)
    {
        return new ClienteView
        {
            Id = cliente.Id.Value,
            CriadoEm = cliente.CriadoEm,
            UltimaAtualizacao = cliente.UltimaAtualizacao,
            NomeCompleto = cliente.NomeCompleto,
            Email = cliente.Email,
            Telefones = cliente.Telefones.Select(t => t.ToViewModel()).ToArray()
        };
    }

    private static TelefoneView ToViewModel(this Telefone telefone)
    {
        return new TelefoneView
        {
            Id = telefone.Id.Value,
            CriadoEm = telefone.CriadoEm,
            UltimaAtualizacao = telefone.UltimaAtualizacao,
            Numero = telefone.Numero,
            Tipo = telefone.Tipo
        };
    }
}