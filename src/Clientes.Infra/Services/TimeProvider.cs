using Clientes.Application.Common;
using Clientes.Domain.Common;

namespace Clientes.Infra.Services;

public sealed class TimeProvider : ITimeProvider
{
    public DateTime Now => DateTime.Now;
}