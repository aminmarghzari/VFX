using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VFX.Domain.Entities;

namespace VFX.Infrastructure.Data.Configurations;

// Configuration class for the Currency entity, implementing IEntityTypeConfiguration<Currency>
public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.ToTable("Currency");

        // Id
        builder.HasKey(x => x.Id);

        // Code
        builder.Property(x => x.Code)
               .IsRequired()
               .HasMaxLength(3);

        // Unique constraint for Code
        builder.HasIndex(x => x.Code)
               .IsUnique();
    }
}

