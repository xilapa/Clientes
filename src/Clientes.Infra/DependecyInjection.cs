using Clientes.Application.Clientes.Commands.CadastrarCliente;
using Clientes.Application.Common;
using Clientes.Application.Common.Behaviours;
using Clientes.Infra.Persistence;
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

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CadastrarClienteCommandValidator>();
        services.AddScoped<ITimeProvider, TimeProvider>();
        services.AddMediator(opts =>
        {
            opts.Namespace = "Clientes.Infra.Mediator";
            opts.ServiceLifetime = ServiceLifetime.Scoped;
        });
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
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