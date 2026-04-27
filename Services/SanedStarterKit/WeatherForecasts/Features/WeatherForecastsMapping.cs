using Mapster;
using SanedStarterKit.WeatherForecasts.Features.CreatingWeatherForecast.V1;
using SanedStarterKit.WeatherForecasts.Features.GettingByIdWeatherForecast.V1;
using SanedStarterKit.WeatherForecasts.Models;

namespace SanedStarterKit.WeatherForecasts.Features;
public class WeatherForecastsMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.Default.NameMatchingStrategy(NameMatchingStrategy.Flexible);

        config.NewConfig<WeatherForecast, GetByIdWeatherForecastResult>().MapToConstructor(true);

        config.NewConfig<CreateWeatherForecast, WeatherForecast>().MapToConstructor(true);
        config.NewConfig<WeatherForecast, WeatherForecastCreatedDomainEvent>().MapToConstructor(true);
        config.NewConfig<WeatherForecast, CreateWeatherForecastResult>().MapToConstructor(true);
    }
}