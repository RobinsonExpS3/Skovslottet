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

            entity.HasOne(g => g.GroceryDay)
                .WithMany(r => r.Residents)
                .HasForeignKey(g => g.GroceryDayID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(m => m.Medicines)
                .WithOne(r => r.Resident)
                .HasForeignKey(r => r.ResidentID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
