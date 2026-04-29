using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations
{
    public class ShiftBoardConfiguration : IEntityTypeConfiguration<ShiftBoard>
    {
        public void Configure(EntityTypeBuilder<ShiftBoard> entity)
        {
            entity.HasKey(sb => new { sb.ShiftBoardID });
            entity.Property(sb => sb.ShiftType)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(sb => sb.StartDateTime)
                .IsRequired();

            entity.Property(sb => sb.EndDateTime)
                .IsRequired();

            entity.HasMany(ss => ss.StaffShifts)
                .WithOne(sb => sb.ShiftBoard)
                .HasForeignKey(ss => ss.ShiftBoardID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
