using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using System.Text.Json;
using VFX.Application.Common;
using VFX.Application.Services;

namespace VFX.Unittest;

// Unit test class for testing AlphavantageApiService
public class AlphavantageApiServiceTests
{
    private Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private AlphavantageApiService _service;
    private AppSettings _appSettings;

    public AlphavantageApiServiceTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

        _appSettings = new AppSettings
        {
            CurrencyExchangeRateProvider = new CurrencyExchangeRateProvider
            {
                CurrencyExchangeRateApiUrl = "https://example.com/api",
                ApiKey = "test_api_key"
            }
        };

        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _service = new AlphavantageApiService(httpClient, _appSettings);
    }

    // Test case to verify that the service correctly returns exchange rates when the API response is valid
    [Fact]
    public async Task GetExchangeRateFromApi_ShouldReturnExchangeRate_WhenResponseIsValid()
    {
        // Arrange
        var jsonResponse = new Dictionary<string, JsonElement>
        {
            {
                "Realtime Currency Exchange Rate", JsonSerializer.SerializeToElement(new Dictionary<string, string>
                {
                    { "1. From_Currency Code", "USD" },
                    { "2. From_Currency Name", "US Dollar" },
                    { "3. To_Currency Code", "EUR" },
                    { "4. To_Currency Name", "Euro" },
                    { "5. Exchange Rate", "1.1" },
                    { "6. Last Refreshed", "2024-11-10 10:00:00" },
                    { "7. Time Zone", "UTC" },
                    { "8. Bid Price", "1.0" },
                    { "9. Ask Price", "1.2" }
                })
            }
        };

        // Mock HTTP response message with OK status and the prepared JSON content
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(jsonResponse), Encoding.UTF8, "application/json")
        };

        // Set up mock for HttpMessageHandler to return the mocked response
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

        // Act
        var result = await _service.GetExchangeRateFromApi("USD", "EUR");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("USD", result.FromCurrencyCode);
        Assert.Equal("EUR", result.ToCurrencyCode);
        Assert.Equal(1.1m, result.ExchangeRate);
        Assert.Equal("2024-11-10 10:00:00", result.LastRefreshed.ToString("yyyy-MM-dd HH:mm:ss"));
    }

    // Test case to verify that the service throws an exception when the API call fails (internal server error)
    [Fact]
    public async Task GetExchangeRateFromApi_ShouldThrowException_WhenApiCallFails()
    {
        // Arrange
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("Error")
        };

        // Set up mock for HttpMessageHandler to return the mocked response
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => _service.GetExchangeRateFromApi("USD", "EUR"));
        Assert.Equal("Failed to retrieve exchange rate from external API.", ex.Message);
    }
}
