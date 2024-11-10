using VFX.Application.Common.Models;
using VFX.Domain.Entities;

namespace VFX.Application.Interface;

public interface IForeignExchangeRateService
{
    Task<ForeignExchangeRateDTO> GetExchangeRate(string currencyPair, CancellationToken token);
    Task<Pagination<ForeignExchangeRateDTO>> Get(int pageIndex, int pageSize);
    Task<ForeignExchangeRateDTO> Get(int id);
    Task Add(ForeignExchangeRateCreateDTO request, CancellationToken token);
    Task Update(ForeignExchangeRateUpdateDTO request, CancellationToken token);
    Task Delete(int id, CancellationToken token);
}
