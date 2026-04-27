using BuildingBlocks.Web;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace PMLogger.WeatherForecasts.Features.GettingByIdWeatherForecast.V1
{

    [Route($"api/{{__tenant__}}/PMLogger/GetByIdWeatherForecast/v1")]
    [ApiController]
    public class GetByIdWeatherForecastEndpoint : ServiceControllerBase
    {
        public GetByIdWeatherForecastEndpoint(ILogger<ServiceControllerBase> logger) : base(logger)
        {
        }
        [HttpPost]
        public async Task<IResult> Post(string __tenant__, [FromBody] GetByIdWeatherForecast request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("API REQ {Endpoint}, {tenant}", HttpContext.Request.Path, __tenant__);
            var validResult = await HttpContext.RequestServices.GetRequiredService<IValidator<GetByIdWeatherForecast>>().ValidateAsync(request, cancellationToken);
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
