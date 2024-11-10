namespace VFX.Application.Common;

// This class holds all the application settings loaded from configuration files (e.g., appsettings.json)
public class AppSettings
{
    public ApplicationDetail ApplicationDetail { get; set; }
    public ConnectionStrings ConnectionStrings { get; set; }
    public Logging Logging { get; set; }
    public bool UseInMemoryDatabase { get; set; }
    public string[] Cors { get; set; }
    public CurrencyExchangeRateProvider CurrencyExchangeRateProvider { get; set; }
    public KafkaSettings KafkaSettings { get; set; }
}

// Application specific details such as name, description, and contact website
public class ApplicationDetail
{
    public string ApplicationName { get; set; }
    public string Description { get; set; }
    public string ContactWebsite { get; set; }
}

// Connection strings for the database
public class ConnectionStrings
{
    public string DefaultConnection { get; set; }
}

// Settings related to the Currency Exchange Rate Provider (Alphavantage) (API URL and key)
public class CurrencyExchangeRateProvider
{
    public string CurrencyExchangeRateApiUrl { get; set; }
    public string ApiKey { get; set; }
}

// Logging configurations, including request and response logging
public class Logging
{
    public RequestResponse RequestResponse { get; set; }
}

// Configuration for logging request and response data
public class RequestResponse
{
    public bool IsEnabled { get; set; }
}

// Kafka related settings
public class KafkaSettings
{
    public string BootstrapServers { get; set; }
    public string AddNewRateTopic { get; set; }
    public bool IsEnabled { get; set; }
}