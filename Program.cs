var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:3000");

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();  // Ensure HTTPS redirection is enabled.

var summaries = new[] 
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// Map the weather forecast endpoint
// This endpoint returns a mock weather forecast with random temperatures and summaries.
app.MapGet("/weatherforecast", () =>
{
    // Custom weather data
    var forecast = new[]
    {
        new WeatherForecast(DateOnly.FromDateTime(DateTime.Now.AddDays(1)), 45, "Balmy"),
        new WeatherForecast(DateOnly.FromDateTime(DateTime.Now.AddDays(2)), 30, "Warm"),
        new WeatherForecast(DateOnly.FromDateTime(DateTime.Now.AddDays(3)), 15, "Scorching"),
        new WeatherForecast(DateOnly.FromDateTime(DateTime.Now.AddDays(4)), 48, "Hot"),
        new WeatherForecast(DateOnly.FromDateTime(DateTime.Now.AddDays(5)), 21, "Sweltering")
    };

    // Custom message to display
    var customMessage = "Hello, see today's weather forecast Have a Good Day Ji. Chanegs aagayooo. Common boy this time it should work boyee:";

    return new
    {
        Message = customMessage,  // Display custom message
        WeatherForecast = forecast  // Weather data
    };
})
.WithName("GetWeatherForecast")
.WithOpenApi();  // Enable OpenAPI support for this endpoint.

app.MapGet("/hello", () => "Hello world from Jenkins CI/CD Pipeline!");  // Simple hello world endpoint for testing.

app.Run();  // Run the application.


// WeatherForecast record to hold weather data
// It contains the date, temperature in Celsius, and the weather summary
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    // Property to convert Celsius to Fahrenheit
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

