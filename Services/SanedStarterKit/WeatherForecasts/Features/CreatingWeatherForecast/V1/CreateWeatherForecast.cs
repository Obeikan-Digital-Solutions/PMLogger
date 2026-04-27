using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SanedStarterKit.Data;
using SanedStarterKit.WeatherForecasts.Models;

namespace SanedStarterKit.WeatherForecasts.Features.CreatingWeatherForecast.V1;

public record CreateWeatherForecast(DateTime Date, int TemperatureC, string? Summary) : ICommand<CreateWeatherForecastResult>, IInternalCommand
{
    public Guid Id { get; init; } = Guid.NewGuid();
}

public record CreateWeatherForecastResult(Guid Id, DateTime Date, int TemperatureC, string? Summary);


public record WeatherForecastCreatedDomainEvent(Guid Id, DateTime Date, int TemperatureC, string? Summary) : IDomainEvent;

public class CreateWeatherForecastValidator : AbstractValidator<CreateWeatherForecast>
{
    public CreateWeatherForecastValidator()
    {
        RuleFor(x => x.Summary).MinimumLength(3);
        //RuleFor()
    }
}

public class CreateWeatherForecastHandler : ICommandHandler<CreateWeatherForecast, CreateWeatherForecastResult>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CreateWeatherForecastHandler> _logger;
    private readonly IMapper _mapper;

    public CreateWeatherForecastHandler(ApplicationDbContext context, ILogger<CreateWeatherForecastHandler> logger, IMapper mapper)
    {
        _context = context;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<CreateWeatherForecastResult> Handle(CreateWeatherForecast request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CommandHandler {Command}, Request {Request}", nameof(CreateWeatherForecast), request);
        Guard.Against.Null(request, nameof(request));
        var exist = await _context.Set<WeatherForecast>().FindAsync(request.Id, cancellationToken);
        if (exist is not null)
        {
            //throw new BuildingBlocks.Exceptions.ServiceDomainException("WeatherForecast Id already exists!");
            throw new Exceptions.WeatherForecastAlreadyExistsException();
        }
        var entry = (await _context.AddAsync(_mapper.Map<WeatherForecast>(request)), cancellationToken).Item1.Entity;

        var _event = _mapper.Map<WeatherForecastCreatedDomainEvent>(entry);
        entry.AddDomainEvent(_event);

        return _mapper.Map<CreateWeatherForecastResult>(entry);
    }
}
