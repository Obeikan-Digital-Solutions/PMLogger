using BuildingBlocks.SanedTenants.Data;
using BuildingBlocks.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PMLogger.WeatherForecasts.EfCong;

namespace PMLogger.Data
{
    public class ApplicationDbContext : BuildingBlocks.EFCore.AppDbContextBase
    {
        public ApplicationDbContext(DbContextOptions options, ICurrentTenantAndUserProvider currentTenantAndUserProvider) : base(options, currentTenantAndUserProvider)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //builder.ApplyConfiguration(new EfConfigWeatherForecast());
            builder.ApplyConfigurationsFromAssembly(typeof(EventMapper).Assembly);
        }
    }
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

            builder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=saned-design-pmlogger;Trusted_Connection=True;MultipleActiveResultSets=true");
            return new ApplicationDbContext(builder.Options, null);
        }
    }
}
