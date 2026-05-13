using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations
{
    public class StaffConfiguration : IEntityTypeConfiguration<Staff>
    {
        /// <summary>
        /// Configures the Entity Framework model for the Staff entity.
        /// </summary>
        /// <param name="entity">The builder used to configure the Staff entity.</param>
        public void Configure(EntityTypeBuilder<Staff> entity)
        {
            entity.HasKey(s => new { s.StaffID });

            entity.Property(s => s.StaffName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(s => s.Initials)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(s => s.Role)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.Department)
                .WithMany(s => s.Staffs)
                .HasForeignKey(d => d.DepartmentID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(ss => ss.StaffShifts)
                .WithOne(s => s.Staff)
                .HasForeignKey(s => s.StaffID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(srs => srs.StaffResidentStatuses)
                .WithOne(s => s.Staff)
                .HasForeignKey(s => s.StaffID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
