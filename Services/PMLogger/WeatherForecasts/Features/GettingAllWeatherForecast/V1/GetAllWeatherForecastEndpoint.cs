using BuildingBlocks.Web;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace PMLogger.WeatherForecasts.Features.GettingAllWeatherForecast.V1
{

    [Route($"api/{{__tenant__}}/PMLogger/GetAllWeatherForecast/v1")]
    [ApiController()]
    public class GetAllWeatherForecastEndpoint : ServiceControllerBase
    {
        public GetAllWeatherForecastEndpoint(ILogger<ServiceControllerBase> logger) : base(logger)
        {
        }
        [HttpGet]
        public async Task<IResult> Post(string __tenant__, [FromBody] GetAllWeatherForecast request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("API REQ {Endpoint}, {tenant}", HttpContext.Request.Path, __tenant__);
            var validResult = await HttpContext.RequestServices.GetRequiredService<IValidator<GetAllWeatherForecast>>().ValidateAsync(request, cancellationToken);
            if (!validResult.IsValid)
            {
                _logger.LogWarning("ValidResult Faild {Endpoint}, {ValidResult}", HttpContext.Request.Path, validResult);
                validResult.AddToModelState(this.ModelState);
                return Results.BadRequest(new ValidationProblemDetails(this.ModelState));
            }
            var result = await Mediator.Send(request, cancellationToken);
            return Results.Json(result);
        }
    }
}
