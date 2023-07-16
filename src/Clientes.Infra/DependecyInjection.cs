using Clientes.Application.Clientes.Commands.CadastrarCliente;
using Clientes.Application.Common.Cache;
using Clientes.Application.Common.Validation;
using Clientes.Domain.Clientes;
using Clientes.Domain.Common;
using Clientes.Infra.Persistence;
using Clientes.Infra.Persistence.Repositories;
using Clientes.Infra.Services;
using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Clientes.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfra(this IServiceCollection services)
    {
        services.AddDbContextPool<ClientesContext>((provider, builder) =>
        {
            var settings = provider.GetRequiredService<IOptions<Settings>>();
            builder.UseSqlite(settings.Value.ConnectionString);

#if DEBUG
            // Código que não deve existir em produção
            builder.EnableSensitiveDataLogging()
                .EnableDetailedErrors()
                .LogTo(Console.WriteLine);
#endif
        });
        services.AddScoped<IClientesContext>(sp => sp.GetRequiredService<ClientesContext>());
        services.AddScoped<IQueryContext>(sp => sp.GetRequiredService<ClientesContext>());
        services.AddScoped<IUow, Uow>();

        services.AddScoped<IClientesRepository, ClientesRepository>();

        services.AddScoped<ITimeProvider, TimeProvider>();
        services.AddSingleton<ICache, MemoryCache>();

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CadastrarClienteCommandValidator>();
        services.AddMediator(opts =>
        {
            opts.Namespace = "Clientes.Infra.Mediator";
            opts.ServiceLifetime = ServiceLifetime.Scoped;
        });
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CacheBehaviour<,>));
        return services;
    }

    public static Task AplicarMigrations(this IServiceProvider serviceProvider)
    {
        return Task.Run(async () =>
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            await using var context = scope.ServiceProvider.GetRequiredService<ClientesContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ClientesContext>>();
            logger.LogInformation("{Data}: Aplicando migrations", DateTime.UtcNow);
            await context.Database.MigrateAsync();
            logger.LogInformation("{Data}: Migrations aplicadas", DateTime.UtcNow);
        });
    }
}