using Azure.Identity;
using Azure.Storage.Blobs;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register BlobServiceClient using DefaultAzureCredential (Managed Identity)
var storageUri = builder.Configuration["Azure:Storage:Uri"];
if (!string.IsNullOrWhiteSpace(storageUri))
{
    builder.Services.AddSingleton(new BlobServiceClient(new Uri(storageUri), new DefaultAzureCredential()));
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

app.MapGet("/storage-check", async (BlobServiceClient? blobServiceClient, ILogger<Program> logger) =>
{
    if (blobServiceClient is null)
    {
        return Results.Ok(new StorageCheckResult(false, "AzureStorageUri is not configured."));
    }

    try
    {
        await blobServiceClient.GetPropertiesAsync();
        return Results.Ok(new StorageCheckResult(true, "Connection to Azure Storage succeeded."));
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Azure Storage connection check failed.");
        return Results.Ok(new StorageCheckResult(false, "Connection to Azure Storage failed. See application logs for details."));
    }
})
.WithName("StorageCheck")
.WithSummary("Check Azure Storage connectivity")
.WithDescription("Returns whether the API can connect to Azure Blob Storage using Managed Identity (DefaultAzureCredential).");

app.MapDefaultEndpoints();

// Liveness probe
app.MapHealthChecks("/healtz", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("live")
});

// Readiness probe
app.MapHealthChecks("/ready");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

record StorageCheckResult(bool Success, string Message);
