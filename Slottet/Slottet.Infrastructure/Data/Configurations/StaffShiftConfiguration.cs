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

            entity.HasOne(ss => ss.ShiftBoard)
                .WithMany(sb => sb.StaffShifts)
                .HasForeignKey(ss => ss.ShiftBoardID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(ss => ss.Staff)
                .WithMany(s => s.StaffShifts)
                .HasForeignKey(ss => ss.StaffID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}