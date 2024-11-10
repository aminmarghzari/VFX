namespace VFX.WebAPI.Middlewares;

// Middleware to handle exceptions globally within the application
// Catches unhandled exceptions during the request pipeline and logs them
public class GlobalExceptionMiddleware(ILoggerFactory logger) : IMiddleware
{
    private readonly ILogger _logger = logger.CreateLogger<GlobalExceptionMiddleware>();
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError("GlobalExceptionMiddleware: {exception}", ex);
            await context.Response.WriteAsync(ex.ToString());
        }
    }
}
