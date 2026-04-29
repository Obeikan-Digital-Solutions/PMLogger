using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PMLogger.PM.Models;

namespace PMLogger.PM.EfCong
{
    public class PmPlanDateChangeConfiguration : IEntityTypeConfiguration<PmPlanDateChange>
    {
        public void Configure(EntityTypeBuilder<PmPlanDateChange> builder)
        {
            builder.ToTable("pm_plan_date_changes");

            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.PlanName).HasColumnName("plan_name").HasMaxLength(255).IsRequired();
            builder.Property(c => c.MachineName).HasColumnName("machine_name").HasMaxLength(255).IsRequired();
            builder.Property(c => c.AreaName).HasColumnName("area_name").HasMaxLength(255);
            builder.Property(c => c.ChecklistType).HasColumnName("checklist_type").HasMaxLength(255);
            builder.Property(c => c.OriginalDate).HasColumnName("original_date");
            builder.Property(c => c.PreviousDate).HasColumnName("previous_date");
            builder.Property(c => c.NewDate).HasColumnName("new_date");
            builder.Property(c => c.ChangedBy).HasColumnName("changed_by").HasMaxLength(255);
            builder.Property(c => c.ChangedAt).HasColumnName("changed_at");
            builder.Property(c => c.Reason).HasColumnName("reason");
        }
    }
}

