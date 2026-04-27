using BuildingBlocks.Core.Event;
using BuildingBlocks.ServiceContracts.IntegrationEvents.StarterKits;

namespace SanedStarterKit
{
    public class EventMapper : IEventMapper
    {
        public IIntegrationEvent MapToIntegrationEvent(IDomainEvent @event)
        {
            return @event switch
            {
                WeatherForecasts.Features.CreatingWeatherForecast.V1.WeatherForecastCreatedDomainEvent e => new WeatherForecastCreatedIntegrationEvent(e.Id, e.Date, e.TemperatureC, e.Summary),
                _ => null
            };
        }
        public IInternalCommand MapToInternalCommand(IDomainEvent @event)
        {
            return @event switch
            {

                _ => null
            };
        }

        public IInternalCommand MapToInternalCommand(IIntegrationEvent @event)
        => @event switch
        {
            _ => null
        };
    }
}
