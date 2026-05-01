using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations
{
    public class RiskLevelConfiguration : IEntityTypeConfiguration<RiskLevel>
    {
        /// <summary>
        /// Configures the Entity Framework model for the RiskLevel entity.
        /// </summary>
        /// <param name="entity">The builder used to configure the RiskLevel entity.</param>
        public void Configure(EntityTypeBuilder<RiskLevel> entity)
        {
            entity.HasKey(rl => rl.RiskLevelID);

            entity.Property(rl => rl.RiskLevelName)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasMany(rl => rl.ResidentStatuses)
                .WithOne(rs => rs.RiskLevel)
                .HasForeignKey(rs => rs.RiskLevelID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
