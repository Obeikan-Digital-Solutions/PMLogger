using Mapster;
using PMLogger.WeatherForecasts.Features.CreatingWeatherForecast.V1;
using PMLogger.WeatherForecasts.Features.GettingByIdWeatherForecast.V1;
using PMLogger.WeatherForecasts.Models;

namespace PMLogger.WeatherForecasts.Features;
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