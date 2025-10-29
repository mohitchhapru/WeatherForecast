using WeatherForecastApplication.Services;
using WeatherForecastApplication.Client;
using WeatherForecastApplication.Common;
using WeatherForecastApplication.Services.Validation;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddScoped<Webcaller>();
builder.Services.AddScoped<OpenMeteoClient>();
builder.Services.AddScoped<WeatherForecastProvider>();
builder.Services.AddScoped<WeatherForecastRequestValidator>();
builder.Services.AddScoped<WeatherService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Weather Forecast API", 
        Version = "v1",
        Description = "An API to get weather forecasts using the Open-Meteo service",
        Contact = new OpenApiContact
        {
            Name = "Weather Forecast Team"
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Weather Forecast API V1");
        c.RoutePrefix = string.Empty; // Serve the Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
