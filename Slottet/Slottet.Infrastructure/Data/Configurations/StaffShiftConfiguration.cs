using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations
{
    public class StaffShiftConfiguration : IEntityTypeConfiguration<StaffShift>
    {
        /// <summary>
        /// Configures the Entity Framework model for the StaffShift entity.
        /// </summary>
        /// <param name="entity">The builder used to configure the StaffShift entity.</param>
        public void Configure(EntityTypeBuilder<StaffShift> entity)
        {
            entity.HasKey(ss => ss.StaffShiftID);

            entity.Property(ss => ss.IsDeleted)
                .IsRequired();

            entity.HasOne(sb => sb.ShiftBoard)
                .WithMany(ss => ss.StaffShifts)
                .HasForeignKey(sb => sb.ShiftBoardID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(s => s.Staff)
                .WithMany(ss => ss.StaffShifts)
                .HasForeignKey(s => s.StaffID)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
