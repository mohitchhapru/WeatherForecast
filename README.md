# WeatherForecast

This project is an ASP.NET Core Web API that fetches weather forecast data from the Open-Meteo service and exposes a local endpoint to query forecasts.

## Required packages (runtime / dev)

The project depends on the following NuGet packages. These are installed in the project file but listed here for clarity.

- Microsoft.AspNetCore.App (provided by the SDK)
- Newtonsoft.Json (for JSON parsing)
- FluentValidation (for request validation)
- Swashbuckle.AspNetCore (Swagger / OpenAPI UI)

If you need to add them manually, run from the project folder:

```bash
dotnet add package Newtonsoft.Json
dotnet add package FluentValidation
dotnet add package Swashbuckle.AspNetCore
```

## Steps to run the project (macOS / zsh)

1. Ensure you have the .NET SDK installed (recommended .NET 9+ for this project). Verify with:

```bash
dotnet --version
```

2. Restore packages and build the solution:

```bash
git clone <>
cd <>
dotnet restore
dotnet build WeatherForcast.slnx -v minimal
```

3. Run the Web API (from the application project folder):

```bash
cd WeatherForcastApplication
dotnet run --project WeatherForcastApplication.csproj
```

By default the app runs in Development and will listen on the port shown in the terminal (for example `http://localhost:5085`).

4. Open Swagger UI to explore the API in your browser:

```
http://localhost:5085/
```

5. Example curl call to the `WeatherForecast` POST endpoint:

```bash
curl -X POST "http://localhost:5085/WeatherForecast/Get" \
	-H "Content-Type: application/json" \
	-d '{
		"latitude": 47.0,
		"longitude": 8.0,
		"hourlyVariables": ["temperature_2m","relativehumidity_2m","weathercode"],
		"dailyVariables": ["temperature_2m_max","temperature_2m_min","precipitation_sum"],
		"temperatureUnit": "celsius",
		"precipitationUnit": "mm",
		"timezone": "Europe/London",
		"forecastDays": 7
	}'
```
# WeatherForecast
