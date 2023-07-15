namespace Clientes.Domain.Common.Erros;

public sealed class Erro
{
    public static Erro NaoEncontrado(params string[] mensagens) => new (TipoErro.NaoEncontrado, mensagens);
    public static Erro Conflito(params string[] mensagens) => new (TipoErro.Conflito, mensagens);
    public static Erro Validacao(params string[] mensagens) => new (TipoErro.Validacao, mensagens);

    public Erro(TipoErro tipoErro, params string[] mensagens)
    {
        TipoErro = tipoErro;
        Mensagens = mensagens;
    }

    public TipoErro TipoErro { get; }
    public string[] Mensagens { get; }
}