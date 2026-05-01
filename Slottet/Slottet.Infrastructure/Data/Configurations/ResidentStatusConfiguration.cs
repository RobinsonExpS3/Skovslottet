using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations
{
    public class ResidentStatusConfiguration : IEntityTypeConfiguration<ResidentStatus>
    {
        /// <summary>
        /// Configures the Entity Framework model for the ResidentStatus entity.
        /// </summary>
        /// <param name="entity">The builder used to configure the ResidentStatus entity.</param>
        public void Configure(EntityTypeBuilder<ResidentStatus> entity)
        {
            entity.HasKey(rs => new { rs.ResidentStatusID });

            entity.Property(rs => rs.Status)
                .IsRequired();

            entity.Property(rs => rs.Date)
                .IsRequired();

            entity.HasOne(r => r.Resident)
                .WithMany(rs => rs.ResidentStatuses)
                .HasForeignKey(r => r.ResidentID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(rl => rl.RiskLevel)
                .WithMany(rs => rs.ResidentStatuses)
                .HasForeignKey(rl => rl.RiskLevelID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
