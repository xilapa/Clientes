using Clientes.Domain.Common.Erros;

namespace Clientes.Domain.Clientes.Erros;

public static class ClienteErros
{
    public static Erro ClienteNaoEncontrado { get; } = Erro.NaoEncontrado("Cliente não encontrado.");
    public static Erro EmailJaCadastrado { get; } = Erro.Conflito("Email já cadastrado.");
    public static Erro TelefoneJaCadastrado { get; } = Erro.Conflito("Telefone já cadastrado.");
    public static Erro TelefoneNaoEncontrado { get; } = Erro.NaoEncontrado("Telefone não encontrado.");
    public static Erro TelefonesJaCadastrados(string[] telefones) =>
        Erro.Conflito("Telefones já cadastrados.", string.Join("; ", telefones));
}