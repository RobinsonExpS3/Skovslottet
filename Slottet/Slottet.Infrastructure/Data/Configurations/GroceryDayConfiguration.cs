using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations
{
    public class GroceryDayConfiguration : IEntityTypeConfiguration<GroceryDay>
    {
        public void Configure(EntityTypeBuilder<GroceryDay> entity)
        {
            entity.HasKey(g => g.GroceryDayID);

            entity.Property(g => g.GroceryDayName)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasMany(g => g.Residents)
                .WithOne(r => r.GroceryDay)
                .HasForeignKey(r => r.GroceryDayID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
