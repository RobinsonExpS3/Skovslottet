using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Seed
{
    public static class StaffShiftSeeder
    {
        public static async Task SeedAsync(SlottetDBContext context)
        {
            if (await context.StaffShifts.AnyAsync())
                return;

            // Link Skoven staff to today's shiftboards
            var skovenStaff = await context.Staffs
                .Include(s => s.Department)
                .Where(s => s.Department.DepartmentName == "Skoven")
                .ToListAsync();

            var todayShifts = await context.ShiftBoards
                .Where(sb => sb.StartDateTime.Date == DateTime.Today ||
                             sb.EndDateTime.Date   == DateTime.Today)
                .ToListAsync();

            if (skovenStaff.Count == 0 || todayShifts.Count == 0)
                return;

            var links = new List<StaffShift>();
            foreach (var shift in todayShifts)
            {
                foreach (var staff in skovenStaff)
                {
                    var exists = await context.StaffShifts
                        .AnyAsync(ss => ss.ShiftBoardID == shift.ShiftBoardID &&
                                        ss.StaffID      == staff.StaffID);
                    if (!exists)
                    {
                        links.Add(new StaffShift
                        {
                            ShiftBoardID = shift.ShiftBoardID,
                            StaffID      = staff.StaffID,
                        });
                    }
                }
            }

            if (links.Count > 0)
            {
                context.StaffShifts.AddRange(links);
                await context.SaveChangesAsync();
            }
        }
    }
}
