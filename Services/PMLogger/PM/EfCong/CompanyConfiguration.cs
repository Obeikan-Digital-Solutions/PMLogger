using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PMLogger.PM.Models;

namespace PMLogger.PM.EfCong
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("companies");

            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.CompanyCode).HasColumnName("company_code").HasMaxLength(100).IsRequired();
            builder.Property(c => c.CompanyName).HasColumnName("company_name").HasMaxLength(255).IsRequired();
            builder.Property(c => c.LogoUrl).HasColumnName("logo_url").HasMaxLength(500);
            builder.Property(c => c.IsActive).HasColumnName("is_active");
            builder.Property(c => c.CreatedBy).HasColumnName("created_by");
            builder.Property(c => c.CreatedAt).HasColumnName("created_at");
        }
    }
}

