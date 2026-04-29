using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PMLogger.PM.Models;

namespace PMLogger.PM.EfCong
{
    public class PmMachineScheduleConfiguration : IEntityTypeConfiguration<PmMachineSchedule>
    {
        public void Configure(EntityTypeBuilder<PmMachineSchedule> builder)
        {
            builder.ToTable("pm_machine_schedules");

            builder.Property(s => s.Id).HasColumnName("id");
            builder.Property(s => s.PlanName).HasColumnName("plan_name").HasMaxLength(255).IsRequired();
            builder.Property(s => s.AreaName).HasColumnName("area_name").HasMaxLength(255).IsRequired();
            builder.Property(s => s.MachineName).HasColumnName("machine_name").HasMaxLength(255).IsRequired();
            builder.Property(s => s.AssignedTechnician).HasColumnName("assigned_technician").HasMaxLength(255);
            builder.Property(s => s.ScheduledDate).HasColumnName("scheduled_date");
            builder.Property(s => s.Shift).HasColumnName("shift").HasMaxLength(50);
            builder.Property(s => s.Priority).HasColumnName("priority").HasMaxLength(50);
            builder.Property(s => s.Notes).HasColumnName("notes");
            builder.Property(s => s.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
            builder.Property(s => s.CreatedAt).HasColumnName("created_at");
            builder.Property(s => s.UpdatedAt).HasColumnName("updated_at");
            builder.Property(s => s.Status).HasColumnName("status").HasMaxLength(50);
            builder.Property(s => s.CompanyId).HasColumnName("company_id");

            builder.HasOne(s => s.Company)
                .WithMany(c => c.PmMachineSchedules)
                .HasForeignKey(s => s.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

