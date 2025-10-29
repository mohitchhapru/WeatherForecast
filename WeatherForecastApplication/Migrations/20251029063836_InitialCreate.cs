using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeatherForecastApplication.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Latitude = table.Column<double>(type: "REAL", precision: 10, scale: 7, nullable: false),
                    Longitude = table.Column<double>(type: "REAL", precision: 10, scale: 7, nullable: false),
                    Elevation = table.Column<double>(type: "REAL", precision: 10, scale: 2, nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    LastAccessedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherForecasts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LocationId = table.Column<int>(type: "INTEGER", nullable: false),
                    ForecastDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RetrievedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Timezone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    TimezoneAbbreviation = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    HourlyDataJson = table.Column<string>(type: "TEXT", nullable: true),
                    DailyDataJson = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentDataJson = table.Column<string>(type: "TEXT", nullable: true),
                    TemperatureMax = table.Column<double>(type: "REAL", precision: 5, scale: 2, nullable: true),
                    TemperatureMin = table.Column<double>(type: "REAL", precision: 5, scale: 2, nullable: true),
                    PrecipitationSum = table.Column<double>(type: "REAL", precision: 6, scale: 2, nullable: true),
                    WeatherCode = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherForecasts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeatherForecasts_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Location_Coordinates",
                table: "Locations",
                columns: new[] { "Latitude", "Longitude" });

            migrationBuilder.CreateIndex(
                name: "IX_WeatherForecast_Location_Date",
                table: "WeatherForecasts",
                columns: new[] { "LocationId", "ForecastDate" });

            migrationBuilder.CreateIndex(
                name: "IX_WeatherForecast_RetrievedAt",
                table: "WeatherForecasts",
                column: "RetrievedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeatherForecasts");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
