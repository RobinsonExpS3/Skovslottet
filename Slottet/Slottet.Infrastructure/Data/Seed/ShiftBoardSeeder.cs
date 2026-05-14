using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Seed
{
    public static class ShiftBoardSeeder
    {
        public static async Task SeedAsync(SlottetDBContext context)
        {
            var defaultDepartment = await context.Departments
                .Where(d => d.DepartmentName == "Skoven")
                .FirstOrDefaultAsync()
                ?? await context.Departments.FirstOrDefaultAsync();

            var startDate = DateTime.Today.AddDays(-30);
            var endDate = startDate.AddDays(60);

            // Backfill existing ShiftBoards that were created before DepartmentID was added
            var shiftsWithoutDepartment = await context.ShiftBoards
                .Where(s => s.DepartmentID == null)
                .ToListAsync();

            foreach (var shift in shiftsWithoutDepartment)
                shift.DepartmentID = defaultDepartment?.DepartmentID;

            if (shiftsWithoutDepartment.Count > 0)
                await context.SaveChangesAsync();

            var shiftsToAdd = new List<ShiftBoard>();

            for (var day = startDate; day < endDate; day = day.AddDays(1))
            {
                var nextDay = day.AddDays(1);

                var dayHasShifts = context.ShiftBoards
                    .Any(s => s.StartDateTime >= day && s.StartDateTime < nextDay);

                if (dayHasShifts)
                    continue;

                shiftsToAdd.Add(new ShiftBoard
                {
                    ShiftBoardID  = Guid.NewGuid(),
                    ShiftType     = "Dag",
                    StartDateTime = day.AddHours(7),
                    EndDateTime   = day.AddHours(15),
                    DepartmentID  = defaultDepartment?.DepartmentID
                });

                shiftsToAdd.Add(new ShiftBoard
                {
                    ShiftBoardID  = Guid.NewGuid(),
                    ShiftType     = "Aften",
                    StartDateTime = day.AddHours(15),
                    EndDateTime   = day.AddHours(23),
                    DepartmentID  = defaultDepartment?.DepartmentID
                });

                shiftsToAdd.Add(new ShiftBoard
                {
                    ShiftBoardID  = Guid.NewGuid(),
                    ShiftType     = "Nat",
                    StartDateTime = day.AddHours(23),
                    EndDateTime   = nextDay.AddHours(7),
                    DepartmentID  = defaultDepartment?.DepartmentID
                });
            }

            if (shiftsToAdd.Count == 0)
                return;

            context.ShiftBoards.AddRange(shiftsToAdd);
            await context.SaveChangesAsync();
        }
    }
}
