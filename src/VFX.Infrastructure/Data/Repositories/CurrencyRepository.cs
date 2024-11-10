using VFX.Application.Interface;
using VFX.Domain.Entities;

namespace VFX.Infrastructure.Data.Repositories;

// Currency repository that extends the generic repository and implements ICurrencyRepository
public class CurrencyRepository(ApplicationDbContext context) : GenericRepository<Currency>(context), ICurrencyRepository
{
}