using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations {
    public class ResidentStatusConfiguration : IEntityTypeConfiguration<ResidentStatus> {
        public void Configure(EntityTypeBuilder<ResidentStatus> entity) {
            entity.HasKey(rs => new { rs.ResidentStatusID });

            entity.Property(rs => rs.Status)
                .IsRequired();

            entity.Property(rs => rs.Date)
                .IsRequired();

            entity.HasOne(r => r.Resident)
                .WithMany(rs => rs.ResidentStatuses)
                .HasForeignKey(r => r.ResidentStatusID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(rl => rl.RiskLevel)
                .WithOne(rs => rs.ResidentStatuses)
                .HasForeignKey(rl => rl.RiskLevelID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(p => p.PN)
                .WithOne(rs => rs.ResidentStatuses)
                .HasForeignKey(p => p.PNID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
