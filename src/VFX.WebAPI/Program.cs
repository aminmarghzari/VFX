using VFX.Application.Common;
using VFX.Application.Common.Exceptions;
using VFX.WebAPI.Extensions;


// Creating a builder to set up the web application
var builder = WebApplication.CreateBuilder(args);


// Reading application settings from configuration (e.g., appsettings.json or environment variables)
// If the settings are not found, an exception is thrown
var configuration = builder.Configuration.Get<AppSettings>()
    ?? throw ProgramException.AppsettingNotSetException();

builder.Services.AddSingleton(configuration);
var app = await builder.ConfigureServices(configuration).ConfigurePipelineAsync(configuration);

// Run the application asynchronously (starts listening for requests)
await app.RunAsync();

// this line for integration test
public partial class Program { }
