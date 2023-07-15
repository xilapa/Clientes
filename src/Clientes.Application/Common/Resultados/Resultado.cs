using Clientes.Domain.Common.Erros;

namespace Clientes.Application.Common.Resultados;

public sealed class Resultado<T> : IResultado where T : class
{
    public Resultado()
    { }

    public Resultado(T valor)
    {
        Valor = valor;
    }

    public Resultado(Erro erro)
    {
        Erro = erro;
    }

    public T? Valor { get; }
    public Erro? Erro { get; private set; }

    public void DefinirErro(Erro erro)
    {
        Erro = erro;
    }
}

public sealed class Resultado : IResultado
{
    public Resultado()
    { }

    public Resultado(Erro erro)
    {
        Erro = erro;
    }

    public Erro? Erro { get; private set; }

    public void DefinirErro(Erro erro)
    {
        Erro = erro;
    }

    public static Resultado Sucesso { get; } = new();
}

public interface IResultado
{
    void DefinirErro(Erro erro);
}