namespace Clientes.Application.Common.Cache;

public static class CacheKeys
{
    public static string ConsultarClientePeloTelefoneQueryPrefix => "Clientes:ConsultarClientePeloTelefoneQuery";
    public static string ConsultarClientePeloTelefoneQuery(string DDD, string telefone) =>
        $"{ConsultarClientePeloTelefoneQueryPrefix}:{DDD}:{telefone}";

    public static string ConsultarClientesQueryPrefix => "Clientes:ConsultarClientesQuery";

    public static string ConsultarClientesQuery(DateTime? ultimoCriadoEm, int take)
    {
        var ultimoCriadoEmString = ultimoCriadoEm != null
            ? ultimoCriadoEm.Value.ToString("yyyy-MM-ddTHH:mm:ss")
            : string.Empty;
        return $"{ConsultarClientesQueryPrefix}:{ultimoCriadoEmString}:{take}";
    }
}