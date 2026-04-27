using BuildingBlocks.Persistence.EfConfig;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SanedStarterKit.WeatherForecasts.Models;

namespace SanedStarterKit.WeatherForecasts.EfCong
{
    public class EfConfigWeatherForecast : DomainEfConfigBase<WeatherForecast>
    {
        public override void Configure(EntityTypeBuilder<WeatherForecast> builder)
        {
            base.Configure(builder);
            //builder.Ignore(f => f.TenantId);
        }
    }
}
