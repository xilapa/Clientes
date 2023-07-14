namespace Clientes.Application.Common.Resultados;

public sealed class ListaPaginada<T>
{
    public int Total { get; set; }
    public T[] Resultados { get; set; } = null!;
}