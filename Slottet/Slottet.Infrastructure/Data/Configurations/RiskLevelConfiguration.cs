using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations
{
    public class RiskLevelConfiguration : IEntityTypeConfiguration<RiskLevel>
    {
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
