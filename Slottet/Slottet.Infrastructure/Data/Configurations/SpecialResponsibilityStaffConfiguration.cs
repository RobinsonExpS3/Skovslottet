using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations
{
    public class SpecialResponsibilityStaffConfiguration : IEntityTypeConfiguration<SpecialResponsibilityStaff>
    {
        public void Configure(EntityTypeBuilder<SpecialResponsibilityStaff> entity)
        {
            entity.HasKey(x => new { x.SpecialResponsibilityID, x.StaffID, x.AssignedAt });

            entity.HasOne(x => x.SpecialResponsibility)
                .WithMany(x => x.SpecialResponsibilityStaffs)
                .HasForeignKey(x => x.SpecialResponsibilityID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Staff)
                .WithMany()
                .HasForeignKey(x => x.StaffID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Department)
                .WithMany()
                .HasForeignKey(x => x.DepartmentID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
