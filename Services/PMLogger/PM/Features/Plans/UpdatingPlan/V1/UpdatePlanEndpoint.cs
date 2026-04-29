using BuildingBlocks.Web;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace PMLogger.PM.Features.Plans.UpdatingPlan.V1
{
    [Route("api/{__tenant__}/PM/UpdatePlan/v1")]
    [ApiController]
    public class UpdatePlanEndpoint : ServiceControllerBase
    {
        public UpdatePlanEndpoint(ILogger<ServiceControllerBase> logger) : base(logger)
        {
        }

        [HttpPut("{id}")]
        public async Task<IResult> Put(string __tenant__, int id, [FromBody] UpdatePlan request, CancellationToken cancellationToken)
        {
            if (id != request.Id)
            {
                return Results.BadRequest(new { error = "Id mismatch" });
            }

            var validResult = await HttpContext.RequestServices.GetRequiredService<IValidator<UpdatePlan>>().ValidateAsync(request, cancellationToken);
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
                return Results.BadRequest(new { error = ex.Message });
            }
        }
    }
}

