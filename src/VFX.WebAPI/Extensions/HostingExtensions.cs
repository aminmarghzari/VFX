using VFX.Application.Common;
using VFX.Application;
using VFX.Infrastructure;
using VFX.Infrastructure.Data;
using VFX.WebAPI.Middlewares;

namespace VFX.WebAPI.Extensions;

// Extension methods for configuring services and middleware pipeline in the WebApplication.
// These methods are used to set up and configure application services, middleware, and other necessary components for the Web API.
public static class HostingExtensions
{
    // Configures the services required for the application, such as adding infrastructure, application services, and Web API services.
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder, AppSettings appsettings)
    {
        // Add infrastructure services (e.g., database, caching, etc.)
        builder.Services.AddInfrastructuresService(appsettings);

        // Add application services (e.g., business logic, use cases)
        builder.Services.AddApplicationService();

        // Add Web API-specific services (e.g., controllers, API configuration)
        builder.Services.AddWebAPIService(appsettings);

        return builder.Build(); // Return the built WebApplication
    }

    // Configures the middleware pipeline for handling requests and responses in the application.
    public static async Task<WebApplication> ConfigurePipelineAsync(this WebApplication app, AppSettings appsettings)
    {
        // Create a logger factory to handle logging in the middleware
        using var loggerFactory = LoggerFactory.Create(builder => { });

        // Create a scope for resolving scoped services
        using var scope = app.Services.CreateScope();

        // If the application is not using an in-memory database, initialize the database
        if (!appsettings.UseInMemoryDatabase)
        {
            var initialize = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
            await initialize.InitializeAsync(); // Initialize the database
        }

        // Set up Swagger for API documentation
        app.UseSwagger();
        app.UseSwaggerUI(setupAction =>
        {
            setupAction.SwaggerEndpoint("/swagger/OpenAPISpecification/swagger.json", "VFX Financial Foreign Exchange Rate Specification");
            setupAction.RoutePrefix = "swagger"; // Set the route for the Swagger UI
        });

        // Enable Cross-Origin Resource Sharing (CORS) for specific origins
        app.UseCors("AllowSpecificOrigin");

        // Use global exception handling middleware
        app.UseMiddleware<GlobalExceptionMiddleware>();

        // Use logging middleware to log each request
        app.UseMiddleware<LoggingMiddleware>();

        // Set up global exception handler
        app.ConfigureExceptionHandler(loggerFactory.CreateLogger("Exceptions"));

        // Use authentication middleware
        app.UseAuthentication();

        // Use authorization middleware
        app.UseAuthorization();

        // Map the controllers to the API routes
        app.MapControllers();

        return app;
    }
}
