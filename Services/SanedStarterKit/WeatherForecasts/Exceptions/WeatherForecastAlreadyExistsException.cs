using BuildingBlocks.Exceptions;

namespace SanedStarterKit.WeatherForecasts.Exceptions
{
    public class WeatherForecastAlreadyExistsException : ServiceDomainException
    {
        public WeatherForecastAlreadyExistsException() : base("WeatherForecast Already Exists!")
        {
        }

        public WeatherForecastAlreadyExistsException(string message) : base(message)
        {
        }

        public WeatherForecastAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
