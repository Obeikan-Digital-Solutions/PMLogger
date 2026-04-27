using BuildingBlocks.Persistence.EfConfig;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PMLogger.WeatherForecasts.Models;

namespace PMLogger.WeatherForecasts.EfCong
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
