using BuildingBlocks.Web;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace PMLogger.PM.Features.Plans.DeletingPlan.V1
{
    [Route("api/{__tenant__}/PM/DeletePlan/v1")]
    [ApiController]
    public class DeletePlanEndpoint : ServiceControllerBase
    {
        public DeletePlanEndpoint(ILogger<ServiceControllerBase> logger) : base(logger)
        {
        }

        [HttpDelete("{id}")]
        public async Task<IResult> Delete(string __tenant__, int id, CancellationToken cancellationToken)
        {
            var request = new DeletePlan(id);
            
            var validResult = await HttpContext.RequestServices.GetRequiredService<IValidator<DeletePlan>>().ValidateAsync(request, cancellationToken);
            if (!validResult.IsValid)
            {
                _logger.LogWarning("Validation Failed {Endpoint}, {ValidResult}", HttpContext.Request.Path, validResult);
                validResult.AddToModelState(this.ModelState);
                return Results.BadRequest(new ValidationProblemDetails(this.ModelState));
            }

            try
            {
                var result = await Mediator.Send(request, cancellationToken);
                return Results.NoContent();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }
    }
}

