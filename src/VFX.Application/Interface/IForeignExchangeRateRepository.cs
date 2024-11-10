using VFX.Domain.Entities;

namespace VFX.Application.Interface;

public interface IForeignExchangeRateRepository : IGenericRepository<ForeignExchangeRate>
{
    Task<ForeignExchangeRate?> GetLatestExchangeRateAsync(string fromCurrencyCode, string toCurrencyCode);
    Task<IEnumerable<ForeignExchangeRate>> GetAllWithRelatedEntitiesAsync();
}
