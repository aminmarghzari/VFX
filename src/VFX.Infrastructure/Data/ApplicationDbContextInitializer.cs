using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace VFX.Infrastructure.Data;

public class ApplicationDbContextInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger _logger;

    public ApplicationDbContextInitializer(ApplicationDbContext context, ILoggerFactory logger)
    {
        _context = context;
        _logger = logger.CreateLogger<ApplicationDbContextInitializer>();
    }

    public async Task InitializeAsync()
    {
        try
        {
            // Apply any pending migrations
            await _context.Database.MigrateAsync();

            // await SeedUser();
        }
        catch (Exception exception)
        {
            // Log any errors related to migrations
            _logger.LogError(exception, "Migration error");
            throw; // Re-throw the exception after logging
        }
    }
}
