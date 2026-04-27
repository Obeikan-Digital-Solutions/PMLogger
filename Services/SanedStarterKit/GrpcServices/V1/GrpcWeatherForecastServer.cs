using BuildingBlocks;
using Grpc.Core;
using MediatR;
using SanedStarterKit.WeatherForecasts.Features.CreatingWeatherForecast.V1;
using SanedStarterKit.WeatherForecasts.Features.GettingByIdWeatherForecast.V1;
using System.Threading;
using App.Grpc.Protobuf.StarterKits.v1;
using App.Grpc.Protobuf.StarterKits.v1.DTO;

namespace SanedStarterKit.GrpcServices.V1
{
    public class GrpcWeatherForecastServer : StarterKitsWeatherForecasts.StarterKitsWeatherForecastsBase
    {
        private readonly ILogger<GrpcWeatherForecastServer> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediator _mediator;

        public GrpcWeatherForecastServer(ILogger<GrpcWeatherForecastServer> logger, IMediator mediator, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _mediator = mediator;
        }
        public override async Task<WeatherForecastRes> Create(CreateWeatherForecastRequest r, ServerCallContext context)
        {
            var request = new CreateWeatherForecast(r.Date.ToDateTime(), r.TemperatureC, r.Summary);
            var result = await _mediator.Send(request);
            return new WeatherForecastRes
            {
                Id = result.Id.ToString(),
                Date = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(result.Date),
                Summary = result.Summary,
                TemperatureC = result.TemperatureC
            };
        }
        public override async Task<WeatherForecastRes> GetById(GetWeatherForecastRequest request, ServerCallContext context)
        {

            var result = await _mediator.Send(new GetByIdWeatherForecast(Guid.Parse(request.Id)));
            return new WeatherForecastRes
            {
                Id = result.Id.ToString(),
                Date = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(result.Date),
                Summary = result.Summary,
                TemperatureC = result.TemperatureC
            };
        }
        public override Task<WeatherForecastListRes> GetAll(GetAllRequest request, ServerCallContext context)
        {
            return base.GetAll(request, context);
        }
    }
}
