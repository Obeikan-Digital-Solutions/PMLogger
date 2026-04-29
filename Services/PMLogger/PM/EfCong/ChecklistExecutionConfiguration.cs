using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PMLogger.PM.Models;

namespace PMLogger.PM.EfCong
{
    public class ChecklistExecutionConfiguration : IEntityTypeConfiguration<ChecklistExecution>
    {
        public void Configure(EntityTypeBuilder<ChecklistExecution> builder)
        {
            builder.ToTable("checklist_executions");

            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.AreaName).HasColumnName("area_name").HasMaxLength(255).IsRequired();
            builder.Property(e => e.MachineName).HasColumnName("machine_name").HasMaxLength(255).IsRequired();
            builder.Property(e => e.ChecklistType).HasColumnName("checklist_type").HasMaxLength(255).IsRequired();
            builder.Property(e => e.Technician).HasColumnName("technician").HasMaxLength(255).IsRequired();
            builder.Property(e => e.ExecutionDate).HasColumnName("execution_date");
            builder.Property(e => e.CompletedItems).HasColumnName("completed_items");
            builder.Property(e => e.Notes).HasColumnName("notes");
            builder.Property(e => e.IsSubmission).HasColumnName("is_submission");
            builder.Property(e => e.CompanyId).HasColumnName("company_id");

            builder.HasOne(e => e.Company)
                .WithMany(c => c.ChecklistExecutions)
                .HasForeignKey(e => e.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => new { e.AreaName, e.MachineName, e.ChecklistType });
        }
    }
}

