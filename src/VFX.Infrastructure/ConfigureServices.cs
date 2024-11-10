using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VFX.Application.Common;
using VFX.Application.Interface;
using VFX.Infrastructure.Data;
using VFX.Infrastructure.Data.Repositories;
using VFX.Infrastructure.Messaging;
using Microsoft.Extensions.Logging;

namespace VFX.Infrastructure;

// This method configures the services for the application, including database context and repositories.
public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructuresService(this IServiceCollection services, AppSettings configuration)
    {
        // Check if the application should use an in-memory database or a SQL Server database
        if (configuration.UseInMemoryDatabase)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("VFXSolutionDB"));
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.ConnectionStrings.DefaultConnection));
        }

        // register services
        services.AddScoped<ICurrencyRepository, CurrencyRepository>();
        services.AddScoped<IForeignExchangeRateRepository, ForeignExchangeRateRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ApplicationDbContextInitializer>();

        // ILogger و KafkaMessageSender
        services.AddScoped<IMessageSender>(provider =>
        {
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<KafkaMessageSender>();
            return new KafkaMessageSender(configuration.KafkaSettings.BootstrapServers, logger);
        });

        return services;
    }
}
