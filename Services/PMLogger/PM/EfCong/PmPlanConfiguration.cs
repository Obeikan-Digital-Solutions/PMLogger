using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PMLogger.PM.Models;

namespace PMLogger.PM.EfCong
{
    public class PmPlanConfiguration : IEntityTypeConfiguration<PmPlan>
    {
        public void Configure(EntityTypeBuilder<PmPlan> builder)
        {
            builder.ToTable("pm_plans");

            builder.Property(p => p.Id).HasColumnName("id");
            builder.Property(p => p.PlanName).HasColumnName("plan_name").HasMaxLength(255).IsRequired();
            builder.Property(p => p.AreaName).HasColumnName("area_name").HasMaxLength(255).IsRequired();
            builder.Property(p => p.MachineName).HasColumnName("machine_name").HasMaxLength(255).IsRequired();
            builder.Property(p => p.ChecklistType).HasColumnName("checklist_type").HasMaxLength(255);
            builder.Property(p => p.ChecklistVersion).HasColumnName("checklist_version");
            builder.Property(p => p.AssignedTechnician).HasColumnName("assigned_technician").HasMaxLength(255);
            builder.Property(p => p.Status).HasColumnName("status").HasMaxLength(50);
            builder.Property(p => p.ScheduledDate).HasColumnName("scheduled_date");
            builder.Property(p => p.PlanStatus).HasColumnName("plan_status").HasMaxLength(50);
            builder.Property(p => p.CreatedAt).HasColumnName("created_at");
            builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");
            builder.Property(p => p.BaselineDate).HasColumnName("baseline_date");
            builder.Property(p => p.Priority).HasColumnName("priority").HasMaxLength(50);
            builder.Property(p => p.Shift).HasColumnName("shift").HasMaxLength(50);
            builder.Property(p => p.Notes).HasColumnName("notes");
            builder.Property(p => p.ExecutionSubmitted).HasColumnName("execution_submitted");
            builder.Property(p => p.ExecutionSubmittedDate).HasColumnName("execution_submitted_date");
            builder.Property(p => p.ExecutionSubmittedBy).HasColumnName("execution_submitted_by").HasMaxLength(255);
            builder.Property(p => p.ExecutedBy).HasColumnName("executed_by").HasMaxLength(255);
            builder.Property(p => p.ExecutedAt).HasColumnName("executed_at");
            builder.Property(p => p.RejectedBy).HasColumnName("rejected_by").HasMaxLength(255);
            builder.Property(p => p.RejectedDate).HasColumnName("rejected_date");
            builder.Property(p => p.RejectionReason).HasColumnName("rejection_reason");
            builder.Property(p => p.OperationApprovedBy).HasColumnName("operation_approved_by").HasMaxLength(255);
            builder.Property(p => p.OperationApprovedDate).HasColumnName("operation_approved_date");
            builder.Property(p => p.SupervisorApprovedBy).HasColumnName("supervisor_approved_by").HasMaxLength(255);
            builder.Property(p => p.SupervisorApprovedDate).HasColumnName("supervisor_approved_date");
            builder.Property(p => p.SupervisorReturnedBy).HasColumnName("supervisor_returned_by").HasMaxLength(255);
            builder.Property(p => p.SupervisorReturnedDate).HasColumnName("supervisor_returned_date");
            builder.Property(p => p.ExecutionComment).HasColumnName("execution_comment");
            builder.Property(p => p.OperationComment).HasColumnName("operation_comment");
            builder.Property(p => p.SupervisorComment).HasColumnName("supervisor_comment");
            builder.Property(p => p.ScheduleId).HasColumnName("schedule_id");
            builder.Property(p => p.CompanyId).HasColumnName("company_id");

            builder.HasOne(p => p.Company)
                .WithMany(c => c.PmPlans)
                .HasForeignKey(p => p.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.PmMachineSchedule)
                .WithMany(s => s.PmPlans)
                .HasForeignKey(p => p.ScheduleId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(p => new { p.CompanyId, p.PlanName });
        }
    }
}

