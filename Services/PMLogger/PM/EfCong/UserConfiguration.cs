using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PMLogger.PM.Models;

namespace PMLogger.PM.EfCong
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.Property(u => u.Id).HasColumnName("id");
            builder.Property(u => u.Username).HasColumnName("username").HasMaxLength(100).IsRequired();
            builder.Property(u => u.Passcode).HasColumnName("passcode").HasMaxLength(255).IsRequired();
            builder.Property(u => u.Role).HasColumnName("role").HasMaxLength(50).IsRequired();
            builder.Property(u => u.Team).HasColumnName("team").HasMaxLength(100);
            builder.Property(u => u.AreaAccess).HasColumnName("area_access").HasMaxLength(500);
            builder.Property(u => u.IsActive).HasColumnName("is_active");
            builder.Property(u => u.CompanyId).HasColumnName("company_id");
            builder.Property(u => u.CreatedAt).HasColumnName("created_at");

            builder.HasOne(u => u.Company)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.CompanyId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(u => u.Username)
                .IsUnique();
        }
    }
}

