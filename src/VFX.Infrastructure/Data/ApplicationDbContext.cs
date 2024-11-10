using Microsoft.EntityFrameworkCore;
using VFX.Domain.Entities;
using VFX.Infrastructure.Data.Configurations;
using VFX.Infrastructure.Extensions;

namespace VFX.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSet properties for entities
    public DbSet<ForeignExchangeRate> ForeignExchangeRates { get; set; }
    public DbSet<Currency> Currencies { get; set; }

    // Apply configurations and seed data
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply custom configurations
        modelBuilder.ApplyConfiguration(new ForeignExchangeRateConfiguration());
        modelBuilder.ApplyConfiguration(new CurrencyConfiguration());

        // Seed the database with initial data
        modelBuilder.Seed();
    }
}
