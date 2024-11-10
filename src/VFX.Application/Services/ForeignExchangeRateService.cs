using AutoMapper;
using VFX.Application.Common.Models;
using VFX.Application.Interface;
using VFX.Domain.Entities;
using VFX.Application.Services.Validation;
using Microsoft.EntityFrameworkCore;
using VFX.Application.Common;

namespace VFX.Application.Services;

public class ForeignExchangeRateService : IForeignExchangeRateService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IAlphavantageApiService _alphavantageApiService;
    private readonly IMessageSender _messageSender;
    private readonly AppSettings _appSettings;

    // Constructor to initialize dependencies
    public ForeignExchangeRateService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IAlphavantageApiService alphavantageApiService,
        IMessageSender messageSender,
        AppSettings appSettings)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _alphavantageApiService = alphavantageApiService;
        _messageSender = messageSender;
        _appSettings = appSettings;
    }

    // Adds a new exchange rate based on the provided data (fromCurrency, toCurrency, exchangeRate, etc.)
    public async Task Add(ForeignExchangeRateCreateDTO request, CancellationToken token)
    {
        // Retrieve currency entities based on the currency codes
        var (fromCurrency, toCurrency) = await GetCurrencies(request.FromCurrencyCode, request.ToCurrencyCode);
        var exchangeRate = CreateForeignExchangeRate(request, fromCurrency, toCurrency);

        // Save the exchange rate and trigger a message if configured
        await SaveExchangeRate(exchangeRate, token);
    }

    // Retrieves an exchange rate, either from the database or an external API if not found in the database
    public async Task<ForeignExchangeRateDTO> GetExchangeRate(string currencyPair, CancellationToken token)
    {
        // Validate the currency pair format
        ForeignExchangeRateValidation.ValidateCurrencyPair(currencyPair);
        var (fromCurrencyCode, toCurrencyCode) = ParseCurrencyPair(currencyPair);

        // Try to retrieve the exchange rate from the database first
        var exchangeRate = await GetExchangeRateFromDatabase(fromCurrencyCode, toCurrencyCode);

        // If exchange rate is found, map it to DTO and return
        if (exchangeRate != null)
        {
            return _mapper.Map<ForeignExchangeRateDTO>(exchangeRate);
        }

        // If not found, fetch from the external API
        var rate = await _alphavantageApiService.GetExchangeRateFromApi(fromCurrencyCode, toCurrencyCode);
        ForeignExchangeRateValidation.ValidateExchangeRate(rate);

        // Create a new exchange rate entity and save it
        return await CreateAndSaveNewExchangeRate(rate, token);
    }

    // Retrieves a paginated list of exchange rates from the database
    public async Task<Pagination<ForeignExchangeRateDTO>> Get(int pageIndex, int pageSize)
    {
        // Fetch paginated data from the database with necessary related entities (FromCurrency, ToCurrency)
        var pagination = await _unitOfWork.ExchangeRateRepository.ToPagination(
            pageIndex: pageIndex,
            pageSize: pageSize,
            orderBy: x => x.FromCurrency, // Order by FromCurrency
            include: query => query.Include(x => x.FromCurrency).Include(x => x.ToCurrency), // Include related entities
            ascending: true // Ensure results are in ascending order
        );

        // Map the fetched data to DTOs and return the pagination object
        var mappedItems = _mapper.Map<List<ForeignExchangeRateDTO>>(pagination.Items);
        return new Pagination<ForeignExchangeRateDTO>(mappedItems, pagination.TotalCount, pageIndex, pageSize);
    }

    // Retrieves a specific exchange rate by ID from the database
    public async Task<ForeignExchangeRateDTO> Get(int id)
    {
        // Try to retrieve the exchange rate by ID
        var exchangeRate = await _unitOfWork.ExchangeRateRepository.FirstOrDefaultAsync(
            x => x.Id == id,
            query => query.Include(x => x.FromCurrency).Include(x => x.ToCurrency)
        );

        // If not found, throw an exception
        if (exchangeRate == null)
        {
            throw new Exception("Exchange rate not found.");
        }

        // Return the mapped exchange rate DTO
        return _mapper.Map<ForeignExchangeRateDTO>(exchangeRate);
    }

    // Updates an existing exchange rate record with new data
    public async Task Update(ForeignExchangeRateUpdateDTO request, CancellationToken token)
    {
        // Retrieve the existing exchange rate record by ID
        var existingExchangeRate = await _unitOfWork.ExchangeRateRepository.FirstOrDefaultAsync(x => x.Id == request.Id);

        // If not found, throw an exception
        if (existingExchangeRate == null)
        {
            throw new Exception("Exchange rate not found.");
        }

        // Update the entity with new values
        existingExchangeRate.BidPrice = request.BidPrice;
        existingExchangeRate.AskPrice = request.AskPrice;
        existingExchangeRate.ExchangeRate = request.ExchangeRate;
        existingExchangeRate.LastRefreshed = DateTime.UtcNow;
        existingExchangeRate.TimeZone = request.TimeZone;

        // Retrieve the associated currency records
        var fromCurrency = await GetCurrencyByCode(request.FromCurrencyCode);
        var toCurrency = await GetCurrencyByCode(request.ToCurrencyCode);

        existingExchangeRate.FromCurrencyId = fromCurrency.Id;
        existingExchangeRate.ToCurrencyId = toCurrency.Id;

        // Execute the transaction to save the changes
        await _unitOfWork.ExecuteTransactionAsync(() => _unitOfWork.ExchangeRateRepository.Update(existingExchangeRate), token);
    }

    // Deletes an exchange rate record by ID
    public async Task Delete(int id, CancellationToken token)
    {
        // Try to retrieve the exchange rate by ID
        var exchangeRate = await _unitOfWork.ExchangeRateRepository.FirstOrDefaultAsync(x => x.Id == id);

        // If not found, throw an exception
        if (exchangeRate == null)
        {
            throw new Exception("Exchange rate not found.");
        }

        // Execute the transaction to delete the exchange rate
        await _unitOfWork.ExecuteTransactionAsync(() => _unitOfWork.ExchangeRateRepository.Delete(exchangeRate), token);
    }

    // Helper method to retrieve currency entities based on currency codes
    private async Task<(Currency fromCurrency, Currency toCurrency)> GetCurrencies(string fromCurrencyCode, string toCurrencyCode)
    {
        // Fetch currencies by their codes
        var fromCurrency = await GetCurrencyByCode(fromCurrencyCode);
        var toCurrency = await GetCurrencyByCode(toCurrencyCode);

        // Validate both currencies
        ForeignExchangeRateValidation.ValidateCurrency(fromCurrency, fromCurrencyCode);
        ForeignExchangeRateValidation.ValidateCurrency(toCurrency, toCurrencyCode);

        return (fromCurrency, toCurrency);
    }

    // Retrieves a currency entity by its code or throws an exception if not found
    private async Task<Currency> GetCurrencyByCode(string currencyCode)
    {
        return await _unitOfWork.CurrencyRepository.FirstOrDefaultAsync(c => c.Code == currencyCode)
               ?? throw new Exception($"Currency with code {currencyCode} not found.");
    }

    // Creates a new ForeignExchangeRate entity from the provided request data
    private ForeignExchangeRate CreateForeignExchangeRate(ForeignExchangeRateCreateDTO request, Currency fromCurrency, Currency toCurrency)
    {
        return new ForeignExchangeRate
        {
            FromCurrencyId = fromCurrency.Id,
            ToCurrencyId = toCurrency.Id,
            ExchangeRate = request.ExchangeRate,
            TimeZone = request.TimeZone,
            BidPrice = request.BidPrice,
            AskPrice = request.AskPrice,
            LastRefreshed = DateTime.UtcNow
        };
    }

    // Saves an exchange rate entity and sends a message if Kafka is enabled
    private async Task SaveExchangeRate(ForeignExchangeRate exchangeRate, CancellationToken token)
    {
        await _unitOfWork.ExecuteTransactionAsync(async () => await _unitOfWork.ExchangeRateRepository.AddAsync(exchangeRate), token);
        await SendExchangeRateMessageAsync(_mapper.Map<ForeignExchangeRateDTO>(exchangeRate));
    }

    // Sends a message to Kafka if messaging is enabled in the settings
    private async Task SendExchangeRateMessageAsync(ForeignExchangeRateDTO exchangeRateDTO)
    {
        if (_appSettings.KafkaSettings.IsEnabled)
        {
            await _messageSender.SendMessageAsync(_appSettings.KafkaSettings.AddNewRateTopic, exchangeRateDTO);
        }
    }

    // Parses a currency pair string (e.g., "USD/EUR") into separate currency codes
    private (string fromCurrencyCode, string toCurrencyCode) ParseCurrencyPair(string currencyPair)
    {
        var currencies = currencyPair.Split('/');
        return (currencies[0].ToUpperInvariant(), currencies[1].ToUpperInvariant());
    }

    // Retrieves an exchange rate from the database based on currency codes, ordered by the last refresh time
    private async Task<ForeignExchangeRate?> GetExchangeRateFromDatabase(string fromCurrencyCode, string toCurrencyCode)
    {
        return await _unitOfWork.ExchangeRateRepository.GetAllWithRelatedEntitiesAsync()
               .ContinueWith(t => t.Result.OrderByDescending(x => x.LastRefreshed).FirstOrDefault(x => x.FromCurrency.Code == fromCurrencyCode && x.ToCurrency.Code == toCurrencyCode));
    }

    // Creates and saves a new exchange rate entity from external API data
    private async Task<ForeignExchangeRateDTO> CreateAndSaveNewExchangeRate(RealtimeCurrencyExchangeRate rate, CancellationToken token)
    {
        var (fromCurrency, toCurrency) = await GetCurrencies(rate.FromCurrencyCode, rate.ToCurrencyCode);
        var newExchangeRate = new ForeignExchangeRate
        {
            FromCurrencyId = fromCurrency.Id,
            ToCurrencyId = toCurrency.Id,
            BidPrice = rate.BidPrice,
            AskPrice = rate.AskPrice,
            ExchangeRate = rate.ExchangeRate,
            LastRefreshed = rate.LastRefreshed,
            TimeZone = rate.TimeZone
        };

        await SaveExchangeRate(newExchangeRate, token);
        newExchangeRate.FromCurrency = fromCurrency;
        newExchangeRate.ToCurrency = toCurrency;
        return _mapper.Map<ForeignExchangeRateDTO>(newExchangeRate);
    }

    // Updates the fields of an existing exchange rate with new data
    private void UpdateExchangeRate(ForeignExchangeRate existingExchangeRate, ForeignExchangeRateDTO request)
    {
        existingExchangeRate.BidPrice = request.BidPrice;
        existingExchangeRate.AskPrice = request.AskPrice;
        existingExchangeRate.ExchangeRate = request.ExchangeRate;
        existingExchangeRate.LastRefreshed = request.LastRefreshed;
        existingExchangeRate.TimeZone = request.TimeZone;
    }
}
