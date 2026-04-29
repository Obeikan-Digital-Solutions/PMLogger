using BuildingBlocks.Web;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace PMLogger.PM.Features.Plans.GettingPlanEditorData.V1
{
    [Route("api/{__tenant__}/PM/GetPlanEditorData/v1")]
    [ApiController]
    public class GetPlanEditorDataEndpoint : ServiceControllerBase
    {
        public GetPlanEditorDataEndpoint(ILogger<ServiceControllerBase> logger) : base(logger)
        {
        }

        [HttpGet]
        public async Task<IResult> Get(string __tenant__, [FromQuery] int? id, CancellationToken cancellationToken)
        {
            var request = new GetPlanEditorData(id);
            
            var validResult = await HttpContext.RequestServices.GetRequiredService<IValidator<GetPlanEditorData>>().ValidateAsync(request, cancellationToken);
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

