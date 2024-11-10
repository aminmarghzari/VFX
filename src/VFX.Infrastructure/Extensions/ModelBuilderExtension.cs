using Microsoft.EntityFrameworkCore;
using VFX.Domain.Entities;

namespace VFX.Infrastructure.Extensions;

public static class ModelBuilderExtension
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Currency>().HasData(
            new Currency
            {
                Id = 1,
                Code = "USD",
                Name = "United States Dollar",
            },
            new Currency
            {
                Id = 2,
                Code = "EUR",
                Name = "Euro",
            },
            new Currency
            {
                Id = 3,
                Code = "CAD",
                Name = "Canadian Dollar",
            },
            new Currency
            {
                Id = 4,
                Code = "CHF",
                Name = "Swiss Franc",
            },
            new Currency
            {
                Id = 5,
                Code = "JPY",
                Name = "Japanese Yen",
            },
            new Currency
            {
                Id = 6,
                Code = "GBP",
                Name = "British Pound Sterling",
            }
         );
    }

}
