using Clientes.Application.Common;

namespace Clientes.Infra.Services;

public sealed class TimeProvider : ITimeProvider
{
    public DateTime Now => DateTime.Now;
}