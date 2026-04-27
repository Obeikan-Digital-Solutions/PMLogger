using BuildingBlocks.EventBus.Attr;
using BuildingBlocks.EventBus.Events;

namespace BuildingBlocks.ServiceContracts.IntegrationEvents.StarterKits;
[EventName(ServicesConstants.StarterKits+"."+nameof(WeatherForecastCreatedIntegrationEvent))]
public record WeatherForecastCreatedIntegrationEvent(Guid Id, DateTime Date, int TemperatureC, string? Summary) : IntegrationEvent;
