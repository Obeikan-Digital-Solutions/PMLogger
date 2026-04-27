using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using FluentValidation;
using PMLogger.Data;

namespace PMLogger.WeatherForecasts.Features.GettingByIdWeatherForecast.V1
{
    public record GetByIdWeatherForecast(Guid Id) : IQuery<GetByIdWeatherForecastResult>;
    public record GetByIdWeatherForecastResult(Guid Id, DateTime Date, int TemperatureC, string? Summary);
    public class GetByIdWeatherForecastValidator : AbstractValidator<GetByIdWeatherForecast>
    {
        public GetByIdWeatherForecastValidator()
        {
            RuleFor(x => x.Id).NotNull().WithMessage("Id is required!");
        }
    }
    public class GetByIdWeatherForecastHandler : IQueryHandler<GetByIdWeatherForecast, GetByIdWeatherForecastResult>
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetByIdWeatherForecastHandler> _logger;

        public GetByIdWeatherForecastHandler(ApplicationDbContext context, ILogger<GetByIdWeatherForecastHandler> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<GetByIdWeatherForecastResult> Handle(GetByIdWeatherForecast request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("QueryHandler {Query}, Request {Request}", nameof(GetByIdWeatherForecast), request);
            Guard.Against.Null(request, nameof(request));
            var entry = await _context.Set<Models.WeatherForecast>().FindAsync(request.Id);
            if (entry == null)
            {
                throw new BuildingBlocks.Exceptions.NotFoundException("RECORD NOT FOUND " + request.Id);
            }
            return _mapper.Map<GetByIdWeatherForecastResult>(entry);
        }
    }
}
