using Microsoft.EntityFrameworkCore;
using VFX.Application.Interface;
using VFX.Domain.Entities;

namespace VFX.Infrastructure.Data.Repositories;

// Repository for ForeignExchangeRate-specific database operations
public class ForeignExchangeRateRepository : GenericRepository<ForeignExchangeRate>, IForeignExchangeRateRepository
{
    private readonly ApplicationDbContext _context;

    public ForeignExchangeRateRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    // Asynchronously retrieves all foreign exchange rates with their related currencies
    public async Task<IEnumerable<ForeignExchangeRate>> GetAllWithRelatedEntitiesAsync()
    {
        return await _context.ForeignExchangeRates
            .Include(x => x.FromCurrency)
            .Include(x => x.ToCurrency)
            .AsNoTracking()
            .ToListAsync();
    }

    // Asynchronously retrieves the latest exchange rate for a specific currency pair (fromCurrencyCode, toCurrencyCode)
    public async Task<ForeignExchangeRate?> GetLatestExchangeRateAsync(string fromCurrencyCode, string toCurrencyCode)
    {
        return await _context.ForeignExchangeRates
            .Include(x => x.FromCurrency)
            .Include(x => x.ToCurrency)
            .AsNoTracking()
            .Where(x => x.FromCurrency.Code == fromCurrencyCode && x.ToCurrency.Code == toCurrencyCode)
            .OrderByDescending(x => x.LastRefreshed)
            .FirstOrDefaultAsync();
    }
}
