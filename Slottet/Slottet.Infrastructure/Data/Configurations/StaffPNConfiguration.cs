using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations
{
    public class StaffPNConfiguration : IEntityTypeConfiguration<StaffPN>
    {
        /// <summary>
        /// Configures the Entity Framework model for the StaffPN entity.
        /// </summary>
        /// <param name="entity">The builder used to configure the StaffPN entity.</param>
        public void Configure(EntityTypeBuilder<StaffPN> entity)
        {
            entity.HasKey(spn => new { spn.StaffID, spn.PNID });

            entity.HasOne(spn => spn.Staff)
                .WithMany(spn => spn.StaffPNs)
                .HasForeignKey(spn => spn.StaffID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(spn => spn.PN)
                .WithMany(spn => spn.StaffPNs)
                .HasForeignKey(spn => spn.PNID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
