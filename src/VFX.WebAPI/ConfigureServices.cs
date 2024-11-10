using VFX.Application.Common;
using VFX.WebAPI.Extensions;
using VFX.WebAPI.Middlewares;

namespace VFX.WebAPI;

// Configures the services needed for the Web API
// Registers necessary services for controllers, authentication, authorization, and more
public static class ConfigureServices
{
    public static IServiceCollection AddWebAPIService(this IServiceCollection services, AppSettings appSettings)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        // Middleware
        services.AddSingleton<GlobalExceptionMiddleware>();

        // Extension classes
        services.AddCorsCustom(appSettings);
        services.AddHttpClient();
        services.AddSwaggerOpenAPI(appSettings);

        return services;
    }
}
