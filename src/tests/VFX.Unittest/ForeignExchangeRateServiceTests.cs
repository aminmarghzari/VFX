using Moq;
using VFX.Application.Services;
using VFX.Application.Interface;
using VFX.Domain.Entities;
using VFX.Application.Common.Models;
using AutoMapper;
using VFX.Application.Common;
using System.Linq.Expressions;

namespace VFX.Unittest;

// Test class for ForeignExchangeRateService
public class ForeignExchangeRateServiceTests
{
    // Mock dependencies for ForeignExchangeRateService
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IAlphavantageApiService> _apiServiceMock;
    private readonly Mock<IMessageSender> _messageSenderMock;
    private readonly Mock<AppSettings> _appSettingsMock;
    private readonly ForeignExchangeRateService _service;
    private readonly AppSettings _appSettings;


    public ForeignExchangeRateServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _apiServiceMock = new Mock<IAlphavantageApiService>();
        _messageSenderMock = new Mock<IMessageSender>();
        _appSettingsMock = new Mock<AppSettings>();

        _appSettings = new AppSettings
        {
            KafkaSettings = new KafkaSettings
            {
                IsEnabled = true,
                AddNewRateTopic = "test_topic"
            }
        };


        _service = new ForeignExchangeRateService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _apiServiceMock.Object,
            _messageSenderMock.Object,
            _appSettings);
    }

    // Test case to verify that the service returns an exchange rate when a valid ID is provided
    [Fact]
    public async Task Get_ShouldReturnExchangeRate_WhenValidId()
    {
        // Arrange
        var id = 1;
        var existingExchangeRate = new ForeignExchangeRate
        {
            Id = id,
            FromCurrency = new Currency { Code = "USD" },
            ToCurrency = new Currency { Code = "EUR" },
            ExchangeRate = 1.1m
        };

        // Mock the repository method to return the exchange rate for the given ID
        _unitOfWorkMock.Setup(u => u.ExchangeRateRepository.FirstOrDefaultAsync(
            It.Is<Expression<Func<ForeignExchangeRate, bool>>>(expr => expr.Compile()(existingExchangeRate)),
            It.IsAny<Func<IQueryable<ForeignExchangeRate>, IQueryable<ForeignExchangeRate>>>()
        )).ReturnsAsync(existingExchangeRate);

        _mapperMock.Setup(m => m.Map<ForeignExchangeRateDTO>(It.IsAny<ForeignExchangeRate>()))
                   .Returns(new ForeignExchangeRateDTO { ExchangeRate = 1.1m });

        // Act
        var result = await _service.Get(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1.1m, result.ExchangeRate);
    }

    // Test case to verify that the service correctly saves a new exchange rate when a valid request is made
    [Fact]
    public async Task Add_ShouldSaveExchangeRate_WhenValidRequest()
    {
        // Arrange
        var request = new ForeignExchangeRateCreateDTO
        {
            FromCurrencyCode = "USD",
            ToCurrencyCode = "EUR",
            ExchangeRate = 1.1m,
            BidPrice = 1.05m,
            AskPrice = 1.15m,
            TimeZone = "UTC"
        };

        var fromCurrency = new Currency { Id = 1, Code = "USD" };
        var toCurrency = new Currency { Id = 2, Code = "EUR" };

        // Set up the mock for CurrencyRepository.
        _unitOfWorkMock.Setup(u => u.CurrencyRepository.FirstOrDefaultAsync(
            It.Is<Expression<Func<Currency, bool>>>(expr => expr.Compile()(fromCurrency)),
            It.IsAny<Func<IQueryable<Currency>, IQueryable<Currency>>>()
        )).ReturnsAsync(fromCurrency);

        _unitOfWorkMock.Setup(u => u.CurrencyRepository.FirstOrDefaultAsync(
            It.Is<Expression<Func<Currency, bool>>>(expr => expr.Compile()(toCurrency)),
            It.IsAny<Func<IQueryable<Currency>, IQueryable<Currency>>>()
        )).ReturnsAsync(toCurrency);

        // Set up the mock for ExecuteTransactionAsync so that AddAsync is invoked.
        _unitOfWorkMock.Setup(u => u.ExecuteTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
                       .Callback<Func<Task>, CancellationToken>(async (func, _) =>
                       {
                           await func();
                       })
                       .Returns(Task.CompletedTask);

        // Set up the mock for AddAsync.
        _unitOfWorkMock.Setup(u => u.ExchangeRateRepository.AddAsync(It.IsAny<ForeignExchangeRate>()))
                       .Returns(Task.CompletedTask); 

        // Act
        await _service.Add(request, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(u => u.ExchangeRateRepository.AddAsync(It.IsAny<ForeignExchangeRate>()), Times.Once);
    }

    // Test case to verify that the service returns an exchange rate from the database when it already exists
    [Fact]
    public async Task GetExchangeRate_ShouldReturnFromDatabase_WhenExchangeRateExists()
    {
        // Arrange
        var currencyPair = "USD/EUR";
        var existingExchangeRate = new ForeignExchangeRate
        {
            FromCurrency = new Currency { Code = "USD" },
            ToCurrency = new Currency { Code = "EUR" },
            ExchangeRate = 1.1m
        };

        _unitOfWorkMock.Setup(u => u.ExchangeRateRepository.GetAllWithRelatedEntitiesAsync())
                       .ReturnsAsync(new List<ForeignExchangeRate> { existingExchangeRate });

        _mapperMock.Setup(m => m.Map<ForeignExchangeRateDTO>(existingExchangeRate))
                   .Returns(new ForeignExchangeRateDTO { ExchangeRate = 1.1m });

        // Act
        var result = await _service.GetExchangeRate(currencyPair, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1.1m, result.ExchangeRate);
    }
}
