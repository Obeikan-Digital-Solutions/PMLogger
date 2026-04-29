using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PMLogger.PM.Models;

namespace PMLogger.PM.EfCong
{
    public class PmMachineConfiguration : IEntityTypeConfiguration<PmMachine>
    {
        public void Configure(EntityTypeBuilder<PmMachine> builder)
        {
            builder.ToTable("pm_machines");

            builder.Property(m => m.Id).HasColumnName("id");
            builder.Property(m => m.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
            builder.Property(m => m.AreaId).HasColumnName("area_id");
            builder.Property(m => m.CompanyId).HasColumnName("company_id");

            builder.HasOne(m => m.PmArea)
                .WithMany(a => a.PmMachines)
                .HasForeignKey(m => m.AreaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(m => m.Company)
                .WithMany(c => c.PmMachines)
                .HasForeignKey(m => m.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

