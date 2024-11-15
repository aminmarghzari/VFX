
# Foreign Exchange Rate API

## Overview
This project provides a Foreign Exchange Rate API for managing currency exchange rates, intended as a Technical Challenge project. It allows users to interact with exchange rates via a RESTful API. The application fetches data from external APIs (such as Alphavantage) and uses Kafka for event-driven processing.

## Requirements
- Docker and Docker Compose (Optional)
- .NET SDK (for development purposes)
- Access to a Kafka broker (either local or remote)  (Optional) Possibility to disable in appsetting
- SQL Server (local or remote)

## Getting Started

### 1. Clone the Repository

First, clone the repository to your local machine:

```bash
git clone <repository_url>
cd <repository_folder>
```

### 2. Configure Docker and Kafka

This project uses Docker to run Kafka and Zookeeper. Follow these steps to set up Kafka using Docker:

#### Docker Setup

1. **Install Docker** on your machine if not already installed. Follow the instructions for your platform on [Docker's official website](https://docs.docker.com/get-docker/).

2. **Use `docker-compose.yml` file in Solution Items**:

```yaml
version: '2'

services:
  zookeeper:
    image: wurstmeister/zookeeper:latest
    ports:
      - "2181:2181"

  kafka:
    image: wurstmeister/kafka:latest
    ports:
      - "9092:9092"
    expose:
      - "9093"
    environment:
      KAFKA_ADVERTISED_LISTENERS: INSIDE://kafka:9093,OUTSIDE://localhost:9092
      KAFKA_LISTENER_SECURITY_PROTOCOL_MAP: INSIDE:PLAINTEXT,OUTSIDE:PLAINTEXT
      KAFKA_LISTENERS: INSIDE://0.0.0.0:9093,OUTSIDE://0.0.0.0:9092
      KAFKA_INTER_BROKER_LISTENER_NAME: INSIDE
      KAFKA_ZOOKEEPER_CONNECT: zookeeper:2181
      KAFKA_CREATE_TOPICS: "vfx_add_new_rate_topic:1:1"
    volumes:
      - /var/run/docker.sock:/var/run/docker.sock
```

3. **Start Docker services** by running the following command:

```bash
docker-compose up -d
```

This command will start both Zookeeper and Kafka containers.

4. **Verify Kafka and Zookeeper** are running by checking Docker logs:

```bash
docker-compose logs -f kafka
```

### 3. Configure Application Settings

The application settings are defined in the `appsettings.json` file. Here’s an example of the configuration:

```json
{
  "ApplicationDetail": {
    "ApplicationName": "Foreign Exchange Rate API",
    "Description": "Foreign Exchange Rate API For HomeWork!",
    "ContactWebsite": "https://www.linkedin.com/in/amin-marghzari/"
  },
  "ConnectionStrings": {
    "DefaultConnection": "uid=sa;pwd=********;Initial Catalog=VFXFinancialDB;Data Source=.\SQL2k19;MultipleActiveResultSets=true;TrustServerCertificate=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    },
    "RequestResponse": {
      "IsEnabled": true
    }
  },
  "CurrencyExchangeRateProvider": {
    "CurrencyExchangeRateApiUrl": "https://www.alphavantage.co/query?function=CURRENCY_EXCHANGE_RATE",
    "ApiKey": "QUCVFKQ0WBA2H0JI3"
  },
  "KafkaSettings": {
    "BootstrapServers": "localhost:9092",
    "AddNewRateTopic": "vfx_add_new_rate_topic",
    "IsEnabled": true
  },
  "cors": [ "http://localhost:3000" ],
  "AllowedHosts": "*",
  "UseInMemoryDatabase": false
}
```

#### Important Configuration Sections:

- **ConnectionStrings**: Set up your database connection here. Ensure your SQL Server instance is running and accessible.
- **CurrencyExchangeRateProvider**: Configures the external API (Alphavantage) used to fetch exchange rates and ApiKey.
- **KafkaSettings**: Configures Kafka. Ensure Kafka is running and replace `localhost:9092` with the correct Kafka broker URL if needed. If you don’t have Kafka, set `"IsEnabled": false` to disable Kafka integration.
- **UseInMemoryDatabase**: Set this to true to switch to an in-memory database if SQL Server is unavailable for testing.

### 4. Running the Application

Once everything is set up:

1. **Build and run the application**:

```bash
dotnet build
dotnet run
```

2. The application should be available at `http://localhost:5205/`.

### 5. Troubleshooting Kafka

If you don’t have Kafka running locally or don’t need Kafka integration, you can disable it by setting `KafkaSettings.IsEnabled` to `false` in the `appsettings.json` file. This will prevent the application from attempting to connect to Kafka.

## Limitations and Possible Improvements

- **Rate Limiting for External APIs**: The current external API (Alphavantage) might have rate limits. Consider implementing a rate-limiting mechanism to avoid hitting these limits and potentially integrating a caching mechanism to reduce the frequency of external API calls.
- **Cache Mechanism for Performance**: To improve performance and avoid repeated API calls, consider adding a caching layer (e.g., using MemoryCache or Redis) to store the exchange rates fetched from external APIs.
- **Error Handling**: Although basic error handling is implemented, adding retries or fallback strategies for external API calls (e.g., Alphavantage) can improve the application's robustness in case of failure.
- **Unit Testing Coverage**: Expand unit tests to cover edge cases, especially for asynchronous tasks and failure scenarios. Add integration tests to ensure the complete flow works correctly with the database and external APIs.
- **Kafka Integration**: Kafka is enabled by default, but if not required, it can be disabled via configuration. Alternatively, a more advanced event-driven system could be implemented to better handle high-volume data.

### Limitations

- **Currency Data**: A limited set of currency records has been added to the `Currency` entity for testing purposes. More currency records need to be added for full functionality.
- **Integration Tests**: While basic unit tests are in place, integration tests could be extended for better test coverage and validation.
- **Cross-Cutting Concerns**: Additional focus is needed on logging, exception handling, Authentication and Authorization, and monitoring. Ensure that logs are appropriately captured, and error handling is robust for both external API calls and internal processes.


## Database Entities and Relationships
The project defines two main entities:

1. **Currency**: Represents a currency with properties like `Id` and `Code`.
2. **ForeignExchangeRate**: Represents an exchange rate between two currencies, where each exchange rate record links to two `Currency` entities (one for the "FromCurrency" and one for the "ToCurrency").

These entities are configured using **AutoMapper** for seamless transformation between entity models and DTOs. The `CurrencyPair` is implemented by adding `FromCurrency` and `ToCurrency` fields in the `ForeignExchangeRate` entity.


## Database Migration

The database schema can be updated using the Entity Framework migration command:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

On the first application run, the necessary database scripts will be executed automatically.

## Core Services and Controllers

- **ForeignExchangeRateService**: The main service responsible for managing exchange rates, handling requests, and integrating with external APIs.
- **AlphavantageApiService**: Service that interacts with the external Alphavantage API to retrieve exchange rates.
- **ForeignExchangeRateController**: The primary controller responsible for handling HTTP requests related to foreign exchange rates. It supports operations like `GET` and `POST` for exchange rates.

### Swagger UI

Swagger is enabled to explore and test the API endpoints. Access the Swagger UI at `http://localhost:5205/swagger`.

## Architecture

This project follows a **Clean Architecture** pattern, ensuring separation of concerns across layers such as:

- **API Layer**: Controllers that expose endpoints to clients.
- **Service Layer**: Contains business logic (e.g., `ForeignExchangeRateService`).
- **Repository Layer**: Uses a generic repository pattern for data access.
- **Unit of Work**: Provides a transaction boundary for database operations.

### Conclusion

This API provides a framework for managing foreign exchange rates. It fetches data from external sources and integrates with Kafka for event-driven processing. The README provides steps for setting up and running the application locally. Future improvements could include enhanced error handling, additional unit tests, and better performance optimization through caching and rate-limiting.



