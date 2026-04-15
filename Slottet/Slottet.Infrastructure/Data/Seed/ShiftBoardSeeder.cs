using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Seed
{
    public static class ShiftBoardSeeder
    {
        public static async Task SeedAsync(SlottetDBContext context)
        {
            var startDate = DateTime.Today;
            var endDate = startDate.AddDays(30);

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
                    ShiftBoardID = Guid.NewGuid(),
                    ShiftType = "Nat",
                    StartDateTime = day,
                    EndDateTime = day.AddHours(6)
                });

                shiftsToAdd.Add(new ShiftBoard
                {
                    ShiftBoardID = Guid.NewGuid(),
                    ShiftType = "Morgen",
                    StartDateTime = day.AddHours(6),
                    EndDateTime = day.AddHours(12)
                });

                shiftsToAdd.Add(new ShiftBoard
                {
                    ShiftBoardID = Guid.NewGuid(),
                    ShiftType = "Eftermiddag",
                    StartDateTime = day.AddHours(12),
                    EndDateTime = day.AddHours(18)
                });

                shiftsToAdd.Add(new ShiftBoard
                {
                    ShiftBoardID = Guid.NewGuid(),
                    ShiftType = "Aften",
                    StartDateTime = day.AddHours(18),
                    EndDateTime = nextDay
                });
            }

            if (shiftsToAdd.Count == 0)
                return;

            context.ShiftBoards.AddRange(shiftsToAdd);
            await context.SaveChangesAsync();
        }
    }
}
