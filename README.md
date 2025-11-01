# WeatherForecast

This project is an ASP.NET Core Web API that fetches weather forecast data from the Open-Meteo service and exposes a local endpoint to query forecasts.

## Required packages (runtime / dev)

The project depends on the following NuGet packages. These are installed in the project file but listed here for clarity.

- Microsoft.AspNetCore.App (provided by the SDK)
- Newtonsoft.Json (for JSON parsing)
- FluentValidation (for request validation)
- Swashbuckle.AspNetCore (Swagger / OpenAPI UI)
- Microsoft.EntityFrameworkCore (ORM framework)
- Microsoft.EntityFrameworkCore.Sqlite (SQLite database provider)
- Microsoft.EntityFrameworkCore.Design (design-time tools for migrations)

If you need to add them manually, run from the project folder:

```bash
dotnet add package Newtonsoft.Json
dotnet add package FluentValidation
dotnet add package Swashbuckle.AspNetCore
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
```

## Steps to run the project (macOS / zsh)

1. Ensure you have the .NET SDK installed (recommended .NET 9+ for this project). Verify with:

```bash
dotnet --version
```

2. Restore packages and build the solution:

```bash
git clone git@github.com:mohitchhapru/WeatherForecast.git
cd WeatherForecast/WeatherForecastApplication
dotnet build WeatherForecastApplication.csproj -v minimal
```

3. Install EF Core tools globally (if not already installed) to run migrations:

```bash
dotnet tool install --global dotnet-ef
```

If you get an error like "Could not execute because the specified command or file was not found" even after installation, you need to add the tools path to your shell profile.

**For macOS/Linux (zsh):**
```bash
# Add to ~/.zshrc
echo 'export PATH="$PATH:$HOME/.dotnet/tools"' >> ~/.zshrc
source ~/.zshrc
```

**For macOS/Linux (bash):**
```bash
# Add to ~/.bash_profile or ~/.bashrc
echo 'export PATH="$PATH:$HOME/.dotnet/tools"' >> ~/.bash_profile
source ~/.bash_profile
```

If already installed, update to the latest version:
```bash
dotnet tool update --global dotnet-ef
```

Verify the installation:
```bash
dotnet ef --version
```

4. Apply database migrations to create the SQLite database and tables:

```bash
# From the WeatherForecastApplication project folder
cd WeatherForecastApplication
dotnet ef database update
```

This will create a `weatherforecast.db` file in the project directory with the `Locations` and `WeatherForecasts` tables.

**Note:** If you see "No migrations were applied. The database is already up to date", the database already exists and is ready to use.

To verify the database was created, check for the file:
```bash
ls -la weatherforecast.db
```

5. Run the Web API (from the application project folder):

```bash
dotnet run --project WeatherForecastApplication.csproj
```

By default the app runs in Development and will listen on the port shown in the terminal (for example `http://localhost:5085`).

6. Open Swagger UI to explore the API in your browser:

```
http://localhost:5085/
```

7. Example curl call to the `WeatherForecast` POST endpoint:

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
