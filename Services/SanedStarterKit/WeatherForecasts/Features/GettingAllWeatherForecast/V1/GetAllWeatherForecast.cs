using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SanedStarterKit.Data;

namespace SanedStarterKit.WeatherForecasts.Features.GettingAllWeatherForecast.V1
{
    public record GetAllWeatherForecast() : IQuery<GetAllWeatherForecastResult>;
    public record WeatherForecastItemDto(Guid Id, DateTime Date, int TemperatureC, string? Summary);
    public record GetAllWeatherForecastResult(List<WeatherForecastItemDto> Items);
    public class GetAllWeatherForecastValidator : AbstractValidator<GetAllWeatherForecast>
    {
        public GetAllWeatherForecastValidator()
        {
            //RuleFor(x => x.Id).NotNull().WithMessage("Id is required!");
        }
    }
    public class GetAllWeatherForecastHandler : IQueryHandler<GetAllWeatherForecast, GetAllWeatherForecastResult>
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetAllWeatherForecastHandler> _logger;

        public GetAllWeatherForecastHandler(ApplicationDbContext context, ILogger<GetAllWeatherForecastHandler> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<GetAllWeatherForecastResult> Handle(GetAllWeatherForecast request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("QueryHandler {Query}, Request {Request}", nameof(GetAllWeatherForecast), request);
            Guard.Against.Null(request, nameof(request));
            var entries = (await _context.Set<Models.WeatherForecast>().ToListAsync()).Select(f=> _mapper.Map<WeatherForecastItemDto>(f)).ToList();

            return new GetAllWeatherForecastResult(entries);
        }
    }
}
