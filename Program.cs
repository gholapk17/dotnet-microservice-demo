var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:3000");

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),  // Random temperature between -20 and 55 Celsius
            summaries[Random.Shared.Next(summaries.Length)]  // Random weather summary
        ))
        .ToArray();
    return forecast;  // Return the generated forecast as a JSON array.
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

