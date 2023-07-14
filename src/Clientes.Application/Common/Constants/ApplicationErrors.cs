namespace Clientes.Application.Common.Constants;

public static class ApplicationErrors
{
    public static string PropriedadeVazia(string prop) =>
        $"{prop} esta vazio(a).";
    public static string PropriedadeComValorInvalido(string prop) =>
        $"{prop} possui um valor inválido.";
    
    public static string PropriedadeComValorInvalido(string prop, string valorAtual) =>
        $"O(A) {prop} '{valorAtual}' é inválido(a).";
}