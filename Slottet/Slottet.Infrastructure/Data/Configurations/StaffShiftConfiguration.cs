using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations
{
    public class StaffShiftConfiguration : IEntityTypeConfiguration<StaffShift>
    {
        public void Configure(EntityTypeBuilder<StaffShift> entity)
        {
            entity.HasKey(ss => new { ss.ShiftBoardID, ss.StaffID });

            entity.HasOne(sb => sb.ShiftBoard)
                .WithMany(ss => ss.StaffShifts)
                .HasForeignKey(sb => sb.ShiftBoardID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(s => s.Staff)
                .WithMany(ss => ss.StaffShifts)
                .HasForeignKey(s => s.StaffID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}