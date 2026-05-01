using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations
{
    public class GroceryDayConfiguration : IEntityTypeConfiguration<GroceryDay>
    {
        /// <summary>
        /// Configures the Entity Framework model for the GroceryDay entity.
        /// </summary>
        /// <param name="entity">The builder used to configure the GroceryDay entity.</param>
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
