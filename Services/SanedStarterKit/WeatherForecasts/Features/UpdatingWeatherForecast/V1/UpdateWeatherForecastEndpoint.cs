using BuildingBlocks.Web;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace SanedStarterKit.WeatherForecasts.Features.UpdatingWeatherForecast.V1
{

    [Route($"api/{{__tenant__}}/SanedStarterKit/UpdateWeatherForecast/v1")]
    [ApiController()]
    public class UpdateWeatherForecastEndpoint : ServiceControllerBase
    {
        public UpdateWeatherForecastEndpoint(ILogger<ServiceControllerBase> logger) : base(logger)
        {
        }
        [HttpPut]
        public async Task<IResult> Post(string __tenant__, [FromBody] UpdateWeatherForecast request, CancellationToken cancellationToken)
        {
            var validResult = await HttpContext.RequestServices.GetRequiredService<IValidator<UpdateWeatherForecast>>().ValidateAsync(request, cancellationToken);
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
