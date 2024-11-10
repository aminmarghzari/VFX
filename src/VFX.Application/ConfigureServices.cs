using Microsoft.Extensions.DependencyInjection;
using VFX.Application.Interface;
using VFX.Application.Services;

namespace VFX.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationService(this IServiceCollection services)
    {
        services.AddScoped<IForeignExchangeRateService, ForeignExchangeRateService>();
        services.AddScoped<IAlphavantageApiService, AlphavantageApiService>();
        return services;
    }
}
