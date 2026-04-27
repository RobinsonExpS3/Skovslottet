using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Seed
{
    public static class StaffShiftSeeder
    {
        public static async Task<List<StaffShift>> SeedAsync(SlottetDBContext context)
        {
            if (await context.StaffShifts.AnyAsync())
                return await context.StaffShifts.ToListAsync();

            var staffShifts = new List<StaffShift>
            {
                new StaffShift { ShiftBoardID = Guid.NewGuid(), StaffID = Guid.NewGuid() },
                new StaffShift { ShiftBoardID = Guid.NewGuid(), StaffID = Guid.NewGuid() },
                new StaffShift { ShiftBoardID = Guid.NewGuid(), StaffID = Guid.NewGuid() },
                new StaffShift { ShiftBoardID = Guid.NewGuid(), StaffID = Guid.NewGuid() },
                new StaffShift { ShiftBoardID = Guid.NewGuid(), StaffID = Guid.NewGuid() }

            };

            context.StaffShifts.AddRange(staffShifts);
            await context.SaveChangesAsync();
            return staffShifts;
        }
    }
}
