using BuildingBlocks;
using Grpc.Core;
using MediatR;
using PMLogger.WeatherForecasts.Features.CreatingWeatherForecast.V1;
using PMLogger.WeatherForecasts.Features.GettingByIdWeatherForecast.V1;
using System.Threading;
using App.Grpc.Protobuf.StarterKits.v0102;
using App.Grpc.Protobuf.StarterKits.v0102.DTO;
using BuildingBlocks.Exceptions;
using PMLogger.WeatherForecasts.Features.GettingAllWeatherForecast.V1;
using BuildingBlocks.UtilsExtensions;

namespace PMLogger.GrpcServices.v0102
{
    public class GrpcWeatherForecastServerV0102 : StarterKitsWeatherForecastsV0102.StarterKitsWeatherForecastsV0102Base
    {
        private readonly ILogger<GrpcWeatherForecastServerV0102> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediator _mediator;

        public GrpcWeatherForecastServerV0102(ILogger<GrpcWeatherForecastServerV0102> logger, IMediator mediator, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _mediator = mediator;
        }
        public override async Task<WeatherForecastResDtoMessageV0102> Create(CreateWeatherForecastRequestV0102 r, ServerCallContext context)
        {
            var res = new WeatherForecastResDtoMessageV0102()
            {
                Status = 200
            };
            try
            {

                var request = new CreateWeatherForecast(r.Date.ToDateTime(), r.TemperatureC, r.Summary);
                var result = await _mediator.Send(request);
                res.Data = new WeatherForecastResV0102
                {
                    Id = result.Id.ToString(),
                    Date = result.Date.ToProtobufTimestampForceUtc(),
                    Summary = result.Summary,
                    TemperatureC = result.TemperatureC
                };

            }
            catch (Exception ex)
            {
                var details = ex.GetExceptionDetails();
                if (details != null)
                {
                    res.Status = details.StatusCode;
                    res.Title = details.Title;
                    if (details.Errors is not null)
                    {
                        var errosStr = Newtonsoft.Json.JsonConvert.SerializeObject(details.Errors);
                        
                        Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string[]>>(errosStr).ToList()
                            .ForEach(f =>
                            {
                                res.Errors.Add(f.Key, string.Join(", ", f.Value));
                            })
                            ;
                    }
                }
            }
            return res;
        }
        public override async Task<WeatherForecastResDtoMessageV0102> GetById(GetWeatherForecastRequestV0102 request, ServerCallContext context)
        {

            var res = new WeatherForecastResDtoMessageV0102()
            {
                Status = 200
            };
            try
            {

                var result = await _mediator.Send(new GetByIdWeatherForecast(Guid.Parse(request.Id)));
                res.Data = new WeatherForecastResV0102
                {
                    Id = result.Id.ToString(),
                    Date = result.Date.ToProtobufTimestampForceUtc(),
                    Summary = result.Summary,
                    TemperatureC = result.TemperatureC
                };
            }
            catch (Exception ex)
            {
                var details = ex.GetExceptionDetails();
                if (details != null)
                {
                    res.Status = details.StatusCode;
                    if (details.Errors is not null)
                    {
                        var errosStr = Newtonsoft.Json.JsonConvert.SerializeObject(details.Errors);
                        Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string[]>>(errosStr).ToList().ForEach(f =>
                        {
                            res.Errors.Add(f.Key, string.Join(", ", f.Value));
                        });
                    }
                }
            }
            return res;
        }
        public async override Task<WeatherForecastListResDtoMessageV0102> GetAll(GetAllRequestV0102 request, ServerCallContext context)
        {
            var res = new WeatherForecastListResDtoMessageV0102()
            {
                Status = 200
            };
            try
            {

                var result = await _mediator.Send(new GetAllWeatherForecast());
                res.Data = new();
                result.Items.ForEach(item =>
                {
                    res.Data.Items.Add(new WeatherForecastResV0102
                    {
                        Id = item.Id.ToString(),
                        Date = item.Date.ToProtobufTimestampForceUtc(),
                        Summary = item.Summary,
                        TemperatureC = item.TemperatureC
                    });
                });
            }
            catch (Exception ex)
            {
                var details = ex.GetExceptionDetails();
                if (details != null)
                {
                    res.Status = details.StatusCode;
                    if (details.Errors is not null)
                    {
                        var errosStr = Newtonsoft.Json.JsonConvert.SerializeObject(details.Errors);
                        Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string[]>>(errosStr).ToList().ForEach(f =>
                        {
                            res.Errors.Add(f.Key, string.Join(", ", f.Value));
                        });
                    }
                }
            }
            return res;
        }
    }
}
