using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PMLogger.Data;
using PMLogger.WeatherForecasts.Models;

namespace PMLogger.WeatherForecasts.Features.UpdatingWeatherForecast.V1;

public record UpdateWeatherForecast(Guid Id,DateTime Date, int TemperatureC, string? Summary) : ICommand<UpdateWeatherForecastResult>, IInternalCommand;

public record UpdateWeatherForecastResult(Guid Id, DateTime Date, int TemperatureC, string? Summary);


public record WeatherForecastUpdatedDomainEvent(Guid Id, DateTime Date, int TemperatureC, string? Summary) : IDomainEvent;

public class UpdateWeatherForecastValidator : AbstractValidator<UpdateWeatherForecast>
{
    public UpdateWeatherForecastValidator()
    {
        RuleFor(x => x.Summary).NotEmpty();
    }
}

public class UpdateWeatherForecastHandler : ICommandHandler<UpdateWeatherForecast, UpdateWeatherForecastResult>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UpdateWeatherForecastHandler> _logger;
    private readonly IMapper _mapper;

    public UpdateWeatherForecastHandler(ApplicationDbContext context, ILogger<UpdateWeatherForecastHandler> logger, IMapper mapper)
    {
        _context = context;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<UpdateWeatherForecastResult> Handle(UpdateWeatherForecast request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CommandHandler {Command}, Request {Request}", nameof(UpdateWeatherForecast), request);
        Guard.Against.Null(request, nameof(request));
        var exist = await _context.Set<WeatherForecast>().FindAsync(request.Id, cancellationToken);
        if (exist is null)
        {
            throw new BuildingBlocks.Exceptions.ServiceDomainException($"Not Found Record with id({request.Id}) !");
        }
        var entry = exist with
        {
            Date = request.Date,
            Summary = request.Summary,
            TemperatureC = request.TemperatureC,
        };
        _context.Entry(exist).CurrentValues.SetValues(entry);


        var _event = _mapper.Map<WeatherForecastUpdatedDomainEvent>(entry);
        entry.AddDomainEvent(_event);

        return _mapper.Map<UpdateWeatherForecastResult>(entry);
    }
}
