using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations
{
    public class StaffResidentStatusConfiguration : IEntityTypeConfiguration<StaffResidentStatus>
    {
        /// <summary>
        /// Configures the Entity Framework model for the StaffResidentStatus entity.
        /// </summary>
        /// <param name="entity">The builder used to configure the StaffResidentStatus entity.</param>
        public void Configure(EntityTypeBuilder<StaffResidentStatus> entity)
        {
            entity.HasKey(srs => srs.StaffResidentStatusID);

            entity.Property(srs => srs.IsDeleted)
                .IsRequired();

            entity.HasOne(srs => srs.Staff)
                .WithMany(s => s.StaffResidentStatuses)
                .HasForeignKey(srs => srs.StaffID)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(srs => srs.ResidentStatus)
                .WithMany(rs => rs.StaffResidentStatuses)
                .HasForeignKey(srs => srs.ResidentStatusID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
