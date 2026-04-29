using BuildingBlocks.Web;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace PMLogger.PM.Features.Plans.GettingPlanById.V1
{
    [Route("api/{__tenant__}/PM/GetPlanById/v1")]
    [ApiController]
    public class GetPlanByIdEndpoint : ServiceControllerBase
    {
        public GetPlanByIdEndpoint(ILogger<ServiceControllerBase> logger) : base(logger)
        {
        }

        [HttpGet("{id}")]
        public async Task<IResult> Get(string __tenant__, int id, CancellationToken cancellationToken)
        {
            var request = new GetPlanById(id);
            
            var validResult = await HttpContext.RequestServices.GetRequiredService<IValidator<GetPlanById>>().ValidateAsync(request, cancellationToken);
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
            catch (Exception ex) when (ex.Message == "PM plan not found")
            {
                return Results.NotFound(new { error = ex.Message });
            }
        }
    }
}

