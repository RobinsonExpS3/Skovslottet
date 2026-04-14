using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations
{
    public class ResidentConfiguration : IEntityTypeConfiguration<Resident>
    {
        public void Configure(EntityTypeBuilder<Resident> entity)
        {
            entity.HasKey(r => new { r.ResidentID });

            entity.Property(r => r.ResidentName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(r => r.IsActive)
                .IsRequired();

            entity.HasOne(r => r.GroceryDay)
                .WithMany(g => g.Residents)
                .HasForeignKey(r => r.GroceryDayID)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(r => r.Medicines)
                .WithOne(m => m.Resident)
                .HasForeignKey(m => m.ResidentID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
