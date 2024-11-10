using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VFX.Domain.Entities;

namespace VFX.Infrastructure.Data.Configurations;

// Configuration class for the ForeignExchangeRate entity, implementing IEntityTypeConfiguration<ForeignExchangeRate>
public class ForeignExchangeRateConfiguration : IEntityTypeConfiguration<ForeignExchangeRate>
{
    public void Configure(EntityTypeBuilder<ForeignExchangeRate> builder)
    {
        builder.ToTable("ForeignExchangeRate");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.FromCurrency)
               .WithMany()
               .HasForeignKey(x => x.FromCurrencyId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ToCurrency)
               .WithMany()
               .HasForeignKey(x => x.ToCurrencyId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
