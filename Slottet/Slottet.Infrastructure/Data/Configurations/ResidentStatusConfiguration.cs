using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations
{
    public class ResidentStatusConfiguration : IEntityTypeConfiguration<ResidentStatus>
    {
        public void Configure(EntityTypeBuilder<ResidentStatus> entity)
        {
            entity.HasKey(rs => new { rs.ResidentStatusID });

            entity.Property(rs => rs.Status)
                .IsRequired();

            entity.Property(rs => rs.Date)
                .IsRequired();

            entity.HasOne(r => r.Resident)
                .WithMany(rs => rs.ResidentStatuses)
                .HasForeignKey(r => r.ResidentStatusID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(rl => rl.RiskLevel)
                .WithMany(rs => rs.ResidentStatuses)
                .HasForeignKey(rl => rl.RiskLevelID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(p => p.PNs)
                .WithOne(rs => rs.ResidentStatus)
                .HasForeignKey(rs => rs.ResidentStatusID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
