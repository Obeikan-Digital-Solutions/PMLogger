using BuildingBlocks.Web;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace PMLogger.PM.Features.Plans.CreatingPlan.V1
{
    [Route("api/{__tenant__}/PM/CreatePlan/v1")]
    [ApiController]
    public class CreatePlanEndpoint : ServiceControllerBase
    {
        public CreatePlanEndpoint(ILogger<ServiceControllerBase> logger) : base(logger)
        {
        }

        [HttpPost]
        public async Task<IResult> Post(string __tenant__, [FromBody] CreatePlan request, CancellationToken cancellationToken)
        {
            var validResult = await HttpContext.RequestServices.GetRequiredService<IValidator<CreatePlan>>().ValidateAsync(request, cancellationToken);
            if (!validResult.IsValid)
            {
                _logger.LogWarning("Validation Failed {Endpoint}, {ValidResult}", HttpContext.Request.Path, validResult);
                validResult.AddToModelState(this.ModelState);
                return Results.BadRequest(new ValidationProblemDetails(this.ModelState));
            }

            try
            {
                var result = await Mediator.Send(request, cancellationToken);
                return Results.Json(result);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("already exists"))
                {
                    return Results.Conflict(new { error = ex.Message });
                }
                return Results.BadRequest(new { error = ex.Message });
            }
        }
    }
}

