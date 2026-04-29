using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PMLogger.PM.Models;

namespace PMLogger.PM.EfCong
{
    public class PmChecklistTypeConfiguration : IEntityTypeConfiguration<PmChecklistType>
    {
        public void Configure(EntityTypeBuilder<PmChecklistType> builder)
        {
            builder.ToTable("pm_checklist_types");

            builder.Property(t => t.Id).HasColumnName("id");
            builder.Property(t => t.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
        }
    }
}

