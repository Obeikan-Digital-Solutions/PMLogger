using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PMLogger.PM.Models;

namespace PMLogger.PM.EfCong
{
    public class PmMachineRunningHourConfiguration : IEntityTypeConfiguration<PmMachineRunningHour>
    {
        public void Configure(EntityTypeBuilder<PmMachineRunningHour> builder)
        {
            builder.ToTable("pm_machine_running_hours");

            builder.Property(h => h.Id).HasColumnName("id");
            builder.Property(h => h.MachineId).HasColumnName("machine_id");
            builder.Property(h => h.MachineName).HasColumnName("machine_name").HasMaxLength(255).IsRequired();
            builder.Property(h => h.AreaId).HasColumnName("area_id");
            builder.Property(h => h.AreaName).HasColumnName("area_name").HasMaxLength(255);
            builder.Property(h => h.CurrentRunningHours).HasColumnName("current_running_hours").HasColumnType("decimal(10,2)");
            builder.Property(h => h.MaxRunningHours).HasColumnName("max_running_hours").HasColumnType("decimal(10,2)");
            builder.Property(h => h.AlarmStatus).HasColumnName("alarm_status").HasMaxLength(20);
            builder.Property(h => h.LastUpdated).HasColumnName("last_updated");
            builder.Property(h => h.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
            builder.Property(h => h.Notes).HasColumnName("notes");
            builder.Property(h => h.CompanyId).HasColumnName("company_id");

            builder.HasOne(h => h.PmMachine)
                .WithMany(m => m.PmMachineRunningHours)
                .HasForeignKey(h => h.MachineId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(h => h.PmArea)
                .WithMany(a => a.PmMachineRunningHours)
                .HasForeignKey(h => h.AreaId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(h => h.Company)
                .WithMany(c => c.PmMachineRunningHours)
                .HasForeignKey(h => h.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(h => new { h.CompanyId, h.MachineId });
        }
    }
}

