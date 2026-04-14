using System;
using System.Collections.Generic;
using System.Text;
using Slottet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
                .HasMaxLength(5);

            entity.Property(s => s.Role)
                .IsRequired()
                .HasMaxLength(25);

            entity.HasOne(ss => ss.StaffShift)
                .WithMany(s => s.Staffs)
                .HasForeignKey(ss => ss.StaffID, ss.ShiftBoardID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Department)
                .WithMany(s => s.Staffs)
                .HasForeignKey(d => d.DepartmentID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
