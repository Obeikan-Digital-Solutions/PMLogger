using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PMLogger.PM.Models;

namespace PMLogger.PM.EfCong
{
    public class PmAreaConfiguration : IEntityTypeConfiguration<PmArea>
    {
        public void Configure(EntityTypeBuilder<PmArea> builder)
        {
            builder.ToTable("pm_areas");

            builder.Property(a => a.Id).HasColumnName("id");
            builder.Property(a => a.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
            builder.Property(a => a.CompanyId).HasColumnName("company_id");

            builder.HasOne(a => a.Company)
                .WithMany(c => c.PmAreas)
                .HasForeignKey(a => a.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

