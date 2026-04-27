using BuildingBlocks.Core.Model;
namespace PMLogger.WeatherForecasts.Models
{
    public record WeatherForecast(DateTime Date, int TemperatureC, string? Summary) : Aggregate<Guid>;
}