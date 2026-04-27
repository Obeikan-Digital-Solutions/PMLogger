using BuildingBlocks.Web;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace PMLogger.WeatherForecasts.Features.CreatingWeatherForecast.V1
{

    [Route($"api/{{__tenant__}}/PMLogger/CreateWeatherForecast/v1")]
    [ApiController()]
    public class CreateWeatherForecastEndpoint : ServiceControllerBase
    {
        public CreateWeatherForecastEndpoint(ILogger<ServiceControllerBase> logger) : base(logger)
        {
        }
        [HttpPost]
        public async Task<IResult> Post(string __tenant__, [FromBody] CreateWeatherForecast request, CancellationToken cancellationToken)
        {
            var validResult = await HttpContext.RequestServices.GetRequiredService<IValidator<CreateWeatherForecast>>().ValidateAsync(request, cancellationToken);
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
