using BuildingBlocks.Web;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace PMLogger.PM.Features.Plans.GettingAllPlans.V1
{
    [Route("api/{__tenant__}/PM/GetAllPlans/v1")]
    [ApiController]
    public class GetAllPlansEndpoint : ServiceControllerBase
    {
        public GetAllPlansEndpoint(ILogger<ServiceControllerBase> logger) : base(logger)
        {
        }

        [HttpGet]
        public async Task<IResult> Get(string __tenant__, [FromQuery] string? planName, [FromQuery] string? status, [FromQuery] int? limit, CancellationToken cancellationToken)
        {
            var request = new GetAllPlans(planName, status, limit);
            
            var validResult = await HttpContext.RequestServices.GetRequiredService<IValidator<GetAllPlans>>().ValidateAsync(request, cancellationToken);
            if (!validResult.IsValid)
            {
                _logger.LogWarning("Validation Failed {Endpoint}, {ValidResult}", HttpContext.Request.Path, validResult);
                validResult.AddToModelState(this.ModelState);
                return Results.BadRequest(new ValidationProblemDetails(this.ModelState));
            }

            var result = await Mediator.Send(request, cancellationToken);
            return Results.Json(result);
        }
    }
}

