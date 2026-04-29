using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PMLogger.PM.Models;

namespace PMLogger.PM.EfCong
{
    public class PmPlanStatusHistoryConfiguration : IEntityTypeConfiguration<PmPlanStatusHistory>
    {
        public void Configure(EntityTypeBuilder<PmPlanStatusHistory> builder)
        {
            builder.ToTable("pm_plan_status_history");

            builder.Property(h => h.Id).HasColumnName("id");
            builder.Property(h => h.PlanId).HasColumnName("plan_id");
            builder.Property(h => h.Action).HasColumnName("action").HasMaxLength(100).IsRequired();
            builder.Property(h => h.FromStatus).HasColumnName("from_status").HasMaxLength(100);
            builder.Property(h => h.ToStatus).HasColumnName("to_status").HasMaxLength(100);
            builder.Property(h => h.Actor).HasColumnName("actor").HasMaxLength(255);
            builder.Property(h => h.ActorRole).HasColumnName("actor_role").HasMaxLength(50);
            builder.Property(h => h.Comment).HasColumnName("comment");
            builder.Property(h => h.Details).HasColumnName("details");
            builder.Property(h => h.CreatedAt).HasColumnName("created_at");

            builder.HasOne(h => h.PmPlan)
                .WithMany(p => p.PmPlanStatusHistories)
                .HasForeignKey(h => h.PlanId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(h => h.PlanId);
        }
    }
}

