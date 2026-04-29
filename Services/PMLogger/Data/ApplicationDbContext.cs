using BuildingBlocks.SanedTenants.Data;
using BuildingBlocks.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PMLogger.WeatherForecasts.EfCong;
using PMLogger.PM.Models;

namespace PMLogger.Data
{
    public class ApplicationDbContext : BuildingBlocks.EFCore.AppDbContextBase
    {
        public ApplicationDbContext(DbContextOptions options, ICurrentTenantAndUserProvider currentTenantAndUserProvider) : base(options, currentTenantAndUserProvider)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<PmPlan> PmPlans { get; set; }
        public DbSet<PmMachine> PmMachines { get; set; }
        public DbSet<PmChecklistItem> PmChecklistItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ChecklistExecution> ChecklistExecutions { get; set; }
        public DbSet<PmArea> PmAreas { get; set; }
        public DbSet<PmChecklistType> PmChecklistTypes { get; set; }
        public DbSet<PmMachineRunningHour> PmMachineRunningHours { get; set; }
        public DbSet<PmMachineRunningHourHistory> PmMachineRunningHourHistories { get; set; }
        public DbSet<PmMachineSchedule> PmMachineSchedules { get; set; }
        public DbSet<PmPlanDateChange> PmPlanDateChanges { get; set; }
        public DbSet<PmPlanStatusHistory> PmPlanStatusHistories { get; set; }
        public DbSet<PmPpeCode> PmPpeCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //builder.ApplyConfiguration(new EfConfigWeatherForecast());
            builder.ApplyConfigurationsFromAssembly(typeof(PmPlan).Assembly);
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
