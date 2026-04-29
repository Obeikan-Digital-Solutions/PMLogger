using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PMLogger.PM.Models;

namespace PMLogger.PM.EfCong
{
    public class PmChecklistItemConfiguration : IEntityTypeConfiguration<PmChecklistItem>
    {
        public void Configure(EntityTypeBuilder<PmChecklistItem> builder)
        {
            builder.ToTable("pm_checklist_items");

            builder.Property(i => i.Id).HasColumnName("id");
            builder.Property(i => i.ItemText).HasColumnName("item_text").IsRequired();
            builder.Property(i => i.SequenceOrder).HasColumnName("sequence_order");
            builder.Property(i => i.DurationMinutes).HasColumnName("duration_minutes");
            builder.Property(i => i.Assembly).HasColumnName("assembly").HasMaxLength(255);
            builder.Property(i => i.Unit).HasColumnName("unit").HasMaxLength(255);
            builder.Property(i => i.LubricantOrTools).HasColumnName("lubricant_or_tools").HasMaxLength(500);
            builder.Property(i => i.PpeCodeId).HasColumnName("ppe_code_id");
            builder.Property(i => i.Frequency).HasColumnName("frequency").HasMaxLength(100);
            builder.Property(i => i.VersionNumber).HasColumnName("version_number");
            builder.Property(i => i.IsNote).HasColumnName("is_note");
            builder.Property(i => i.IsLubricationSummary).HasColumnName("is_lubrication_summary");
            builder.Property(i => i.AreaId).HasColumnName("area_id");
            builder.Property(i => i.MachineId).HasColumnName("machine_id");
            builder.Property(i => i.ChecklistTypeId).HasColumnName("checklist_type_id");
            builder.Property(i => i.CompanyId).HasColumnName("company_id");
            builder.Property(i => i.CreatedAt).HasColumnName("created_at");
            builder.Property(i => i.UpdatedAt).HasColumnName("updated_at");

            builder.HasOne(i => i.PmArea)
                .WithMany(a => a.PmChecklistItems)
                .HasForeignKey(i => i.AreaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.PmMachine)
                .WithMany(m => m.PmChecklistItems)
                .HasForeignKey(i => i.MachineId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.PmChecklistType)
                .WithMany(t => t.PmChecklistItems)
                .HasForeignKey(i => i.ChecklistTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.Company)
                .WithMany(c => c.PmChecklistItems)
                .HasForeignKey(i => i.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.PmPpeCode)
                .WithMany(p => p.PmChecklistItems)
                .HasForeignKey(i => i.PpeCodeId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(i => new { i.AreaId, i.MachineId, i.ChecklistTypeId, i.VersionNumber });
        }
    }
}

