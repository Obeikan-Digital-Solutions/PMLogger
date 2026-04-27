using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Mapster;
using BuildingBlocks.Web;
using FluentValidation;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.WebUtilities;
using SanedStarterKit.GrpcServices.v0102;
using Microsoft.EntityFrameworkCore;
using SanedStarterKit.Data;
using SanedStarterKit.GrpcServices.V1;
using BuildingBlocks.SanedTenants;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true);
var env = builder.Environment;
builder.AddCustomSerilog(env);
// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddCustomDbContext<ApplicationDbContext>();
builder.Services.AddCustomMapster(typeof(Program).Assembly);

builder.Services.AddControllers();

builder.Services.AddScoped<IEventMapper, EventMapper>();
builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();
builder.Services.AddCustomEventBus(builder.Configuration, typeof(Program).Assembly);
builder.Services.AddMediatR(conf => {
    conf.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(EfTxBehavior<,>));
builder.Services.AddValidatorsFromAssembly(typeof(EventMapper).Assembly);
builder.Services.AddScoped<ICurrentTenantAndUserProvider, CurrentTenantAndUserProvider>();
builder.Services.AddTransient<GrpcExceptionInterceptor>();
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<GrpcExceptionInterceptor>();
});

builder.Services.AddProblemDetails();
builder.Services.AddCustomMultiTenant(builder.Configuration, env);

var app = builder.Build();

app.UseCustom_01_AfterBuild(typeof(Program).Assembly);
app.MapGrpcService<GrpcWeatherForecastServer>();
app.MapGrpcService<GrpcWeatherForecastServerV0102>();

app.UseCorrelationId();
app.UseCustom_02_AfterMappingGrpcs(typeof(Program).Assembly);
app.UseAuthentication();
app.UseAuthorization();

app.UseMigration<ApplicationDbContext>(app.Environment);

app.MapControllers();
app.UseStatusCodePages(statusCodeHandlerApp =>
{
    statusCodeHandlerApp.Run(async context =>
    {
        context.Response.ContentType = "application/problem+json";

        if (context.RequestServices.GetService<IProblemDetailsService>() is { } problemDetailsService)
        {
            await problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails =
                        {
                            Detail = ReasonPhrases.GetReasonPhrase(context.Response.StatusCode),
                            Status = context.Response.StatusCode
                        }
            });
        }
    });
});

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.ContentType = "application/problem+json";

        if (context.RequestServices.GetService<IProblemDetailsService>() is { } problemDetailsService)
        {
            var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
            var exceptionType = exceptionHandlerFeature?.Error;
            object errors = null;
            if (exceptionType is not null)
            {
                (string Detail, string Title, int StatusCode) details = exceptionType switch
                {
                    ConflictException =>
                    (
                        exceptionType.Message,
                        exceptionType.GetType().Name,
                        context.Response.StatusCode = StatusCodes.Status409Conflict
                    ),
                    BuildingBlocks.Exceptions.ValidationException validationException =>
                    (
                        exceptionType.Message,
                        exceptionType.GetType().Name,
                        context.Response.StatusCode = (int)validationException.StatusCode
                    ),
                    BadRequestException =>
                    (
                        exceptionType.Message,
                        exceptionType.GetType().Name,
                        context.Response.StatusCode = StatusCodes.Status400BadRequest
                    ),
                    NotFoundException =>
                    (
                        exceptionType.Message,
                        exceptionType.GetType().Name,
                        context.Response.StatusCode = StatusCodes.Status404NotFound
                    ),
                    AppException =>
                    (
                        exceptionType.Message,
                        exceptionType.GetType().Name,
                        context.Response.StatusCode = StatusCodes.Status400BadRequest
                    ),
                    ServiceDomainException =>
                    (
                        exceptionType.Message,
                        exceptionType.GetType().Name,
                        context.Response.StatusCode = StatusCodes.Status400BadRequest
                    ),
                    DbUpdateConcurrencyException =>
                    (
                        exceptionType.Message,
                        exceptionType.GetType().Name,
                        context.Response.StatusCode = StatusCodes.Status409Conflict
                    ),
                    RpcException =>
                    (
                        exceptionType.Message,
                        exceptionType.GetType().Name,
                        context.Response.StatusCode = StatusCodes.Status400BadRequest
                    ),
                    _ =>
                    (
                        exceptionType.Message,
                        exceptionType.GetType().Name,
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError
                    )
                };

                var problem = new ProblemDetailsContext
                {
                    HttpContext = context,
                    ProblemDetails =
                            {
                                Title = details.Title,
                                Detail = details.Detail,
                                Status = details.StatusCode,
                            }
                };
                var isCustomException = exceptionType?.GetType().IsAssignableTo(typeof(CustomException));
                if (isCustomException.HasValue && isCustomException.Value)
                {
                    var customException = exceptionType as CustomException;
                    problem.ProblemDetails.Extensions.Add("errors", customException.Errors);
                }
                if (app.Environment.IsDevelopment())
                {
                    problem.ProblemDetails.Extensions.Add("exception", exceptionHandlerFeature?.Error.ToString());
                }


                await problemDetailsService.WriteAsync(problem);
            }
        }
    });
});



//if (!app.Environment.IsDevelopment())
{

    app.UseMigrationForAllTenant<ApplicationDbContext>(f => new ApplicationDbContext(f.Options, f.TenantInfo));
    app.UseCustomSeedersForAllTenant<ApplicationDbContext>(f => new ApplicationDbContext(f.Options, f.TenantInfo));
}
app.UseCustomEventBus(typeof(Program).Assembly);
app.UseCustom_03_BeforeRun(typeof(Program).Assembly);
app.Run();

