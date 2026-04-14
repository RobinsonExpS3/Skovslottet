using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations
{
    public class StaffConfiguration : IEntityTypeConfiguration<Staff>
    {
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

            entity.HasOne(s => s.Department)
                .WithMany(d => d.Staffs)
                .HasForeignKey(s => s.DepartmentID)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(s => s.StaffShifts)
                .WithOne(ss => ss.Staff)
                .HasForeignKey(ss => ss.StaffID)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(s => s.StaffResidentStatuses)
                .WithOne(srs => srs.Staff)
                .HasForeignKey(srs => srs.StaffID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
