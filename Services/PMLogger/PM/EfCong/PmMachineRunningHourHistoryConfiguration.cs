using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PMLogger.PM.Models;

namespace PMLogger.PM.EfCong
{
    public class PmMachineRunningHourHistoryConfiguration : IEntityTypeConfiguration<PmMachineRunningHourHistory>
    {
        public void Configure(EntityTypeBuilder<PmMachineRunningHourHistory> builder)
        {
            builder.ToTable("pm_machine_running_hours_history");

            builder.Property(h => h.Id).HasColumnName("id");
            builder.Property(h => h.MachineId).HasColumnName("machine_id");
            builder.Property(h => h.MachineName).HasColumnName("machine_name").HasMaxLength(255).IsRequired();
            builder.Property(h => h.RunningHours).HasColumnName("running_hours").HasColumnType("decimal(10,2)");
            builder.Property(h => h.MaxRunningHours).HasColumnName("max_running_hours").HasColumnType("decimal(10,2)");
            builder.Property(h => h.RecordedDate).HasColumnName("recorded_date");
            builder.Property(h => h.RecordedBy).HasColumnName("recorded_by").HasMaxLength(255);

            builder.HasOne(h => h.PmMachineRunningHour)
                .WithMany(rh => rh.PmMachineRunningHourHistories)
                .HasForeignKey(h => h.MachineId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

