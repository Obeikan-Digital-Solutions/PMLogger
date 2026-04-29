using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PMLogger.PM.Models;

namespace PMLogger.PM.EfCong
{
    public class PmPpeCodeConfiguration : IEntityTypeConfiguration<PmPpeCode>
    {
        public void Configure(EntityTypeBuilder<PmPpeCode> builder)
        {
            builder.ToTable("pm_ppe_codes");

            builder.Property(p => p.Id).HasColumnName("id");
            builder.Property(p => p.Code).HasColumnName("code").HasMaxLength(50).IsRequired();
            builder.Property(p => p.Description).HasColumnName("description").HasMaxLength(255);
        }
    }
}

