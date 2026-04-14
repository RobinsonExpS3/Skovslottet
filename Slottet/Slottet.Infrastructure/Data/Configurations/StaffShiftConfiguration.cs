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

            entity.HasMany(s => s.Staffs)
                .WithOne(s => s.Staff)
                .HasForeignKey(s => s.Staff)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(sb => sb.ShiftBoards)
                .WithOne(s => s.ShiftBoard)
                .HasForeignKey(sb => sb.ShiftBoardID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}