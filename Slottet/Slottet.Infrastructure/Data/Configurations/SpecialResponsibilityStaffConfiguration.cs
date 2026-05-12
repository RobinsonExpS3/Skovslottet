using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations
{
    public class SpecialResponsibilityStaffConfiguration : IEntityTypeConfiguration<SpecialResponsibilityStaff>
    {
        /// <summary>
        /// Configures the Entity Framework model for the SpecialResponsibilityStaff entity.
        /// </summary>
        /// <param name="entity">The builder used to configure the SpecialResponsibilityStaff entity.</param>
        public void Configure(EntityTypeBuilder<SpecialResponsibilityStaff> entity)
        {
            entity.ToTable("SpecialResponsibilityStaff");

            entity.HasKey(x => x.SpecialResponsibilityStaffID);

            entity.Property(x => x.IsDeleted)
                .IsRequired();

            entity.HasOne(x => x.SpecialResponsibility)
                .WithMany(x => x.SpecialResponsibilityStaffs)
                .HasForeignKey(x => x.SpecialResponsibilityID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Staff)
                .WithMany()
                .HasForeignKey(x => x.StaffID)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(x => x.Department)
                .WithMany()
                .HasForeignKey(x => x.DepartmentID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
