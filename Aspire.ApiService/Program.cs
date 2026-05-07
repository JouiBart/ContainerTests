using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Config values are read from environment variables (e.g. Azure__Storage__Uri in AKS)
// with fallback to appsettings.json for local development.
var storageUri = builder.Configuration["Azure:Storage:Uri"];
var keyVaultUri = builder.Configuration["Azure:KeyVault:Uri"];
var dbConnectionString = builder.Configuration["Database:ConnectionString"];

if (!string.IsNullOrWhiteSpace(storageUri))
{
    builder.Services.AddSingleton(new BlobServiceClient(new Uri(storageUri), new DefaultAzureCredential()));
}

if (!string.IsNullOrWhiteSpace(keyVaultUri))
{
    builder.Services.AddSingleton(new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential()));
}

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

app.MapGet("/", () => "API service is running. Navigate to /weatherforecast to see sample data.");

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/services-status", async (
    BlobServiceClient? blobServiceClient,
    SecretClient? secretClient,
    ILogger<Program> logger) =>
{
    var storage = await CheckStorageAsync(blobServiceClient, storageUri, logger);
    var keyVault = await CheckKeyVaultAsync(secretClient, keyVaultUri, logger);
    var database = await CheckDatabaseAsync(dbConnectionString, logger);

    return Results.Ok(new ServicesStatusResult(storage, keyVault, database));
})
.WithName("ServicesStatus")
.WithSummary("Check Azure services connectivity")
.WithDescription("Returns connectivity status for Azure Storage, Azure Key Vault, and the database. Config is read from environment variables (Azure__Storage__Uri, Azure__KeyVault__Uri, Database__ConnectionString) with appsettings.json as fallback.");

app.MapDefaultEndpoints();

// Liveness probe
app.MapHealthChecks("/healtz", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("live")
});

// Readiness probe
app.MapHealthChecks("/ready");

app.Run();

static async Task<ServiceStatus> CheckStorageAsync(BlobServiceClient? client, string? configuredUri, ILogger logger)
{
    if (client is null || string.IsNullOrWhiteSpace(configuredUri))
        return new ServiceStatus(false, false, "Not configured.");

    try
    {
        await client.GetPropertiesAsync();
        return new ServiceStatus(true, true, "Connection succeeded.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Azure Storage connection check failed.");
        return new ServiceStatus(true, false, "Connection failed. See application logs for details.");
    }
}

static async Task<ServiceStatus> CheckKeyVaultAsync(SecretClient? client, string? configuredUri, ILogger logger)
{
    if (client is null || string.IsNullOrWhiteSpace(configuredUri))
        return new ServiceStatus(false, false, "Not configured.");

    try
    {
        // Fetch only the first page with a single item to verify connectivity with minimal overhead.
        var enumerator = client.GetPropertiesOfSecretsAsync().AsPages(pageSizeHint: 1).GetAsyncEnumerator();
        await using (enumerator.ConfigureAwait(false))
        {
            await enumerator.MoveNextAsync();
        }
        return new ServiceStatus(true, true, "Connection succeeded.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Azure Key Vault connection check failed.");
        return new ServiceStatus(true, false, "Connection failed. See application logs for details.");
    }
}

static async Task<ServiceStatus> CheckDatabaseAsync(string? connectionString, ILogger logger)
{
    if (string.IsNullOrWhiteSpace(connectionString))
        return new ServiceStatus(false, false, "Not configured.");

    try
    {
        // The connection string is only used here to open/close a test connection.
        // It is never logged to avoid leaking credentials.
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        return new ServiceStatus(true, true, "Connection succeeded.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database connection check failed.");
        return new ServiceStatus(true, false, "Connection failed. See application logs for details.");
    }
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

// Configured: whether the config value is present; Connected: whether the connection test passed.
record ServiceStatus(bool Configured, bool Connected, string Message);

record ServicesStatusResult(ServiceStatus AzureStorage, ServiceStatus AzureKeyVault, ServiceStatus Database);
