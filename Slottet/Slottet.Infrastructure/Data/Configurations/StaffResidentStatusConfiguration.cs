using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations
{
    public class StaffResidentStatusConfiguration : IEntityTypeConfiguration<StaffResidentStatus>
    {
        public void Configure(EntityTypeBuilder<StaffResidentStatus> entity)
        {
            entity.HasKey(srs => new { srs.StaffID, srs.ResidentStatusID });

            entity.HasOne(srs => srs.Staff)
                .WithMany(s => s.StaffResidentStatuses)
                .HasForeignKey(srs => srs.StaffID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(srs => srs.ResidentStatus)
            .WithMany(sb => sb.StaffResidentStatuses)
            .HasForeignKey(srs => srs.ResidentStatusID)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
