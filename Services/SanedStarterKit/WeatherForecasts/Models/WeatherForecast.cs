using BuildingBlocks.Core.Model;
namespace SanedStarterKit.WeatherForecasts.Models
{
    public record WeatherForecast(DateTime Date, int TemperatureC, string? Summary) : Aggregate<Guid>;
}