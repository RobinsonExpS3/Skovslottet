using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Seed
{
    public static class ShiftBoardSeeder
    {
        public static async Task SeedAsync(SlottetDBContext context)
        {
            var startDate = DateTime.Today.AddDays(-30);
            var endDate = startDate.AddDays(60);

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
                    ShiftType = "Dag",
                    StartDateTime = day.AddHours(7),
                    EndDateTime = day.AddHours(15)
                });

                shiftsToAdd.Add(new ShiftBoard
                {
                    ShiftBoardID = Guid.NewGuid(),
                    ShiftType = "Aften",
                    StartDateTime = day.AddHours(15),
                    EndDateTime = day.AddHours(23)
                });

                shiftsToAdd.Add(new ShiftBoard
                {
                    ShiftBoardID = Guid.NewGuid(),
                    ShiftType = "Nat",
                    StartDateTime = day.AddHours(23),
                    EndDateTime = nextDay.AddHours(7)
                });
            }

            if (shiftsToAdd.Count == 0)
                return;

            context.ShiftBoards.AddRange(shiftsToAdd);
            await context.SaveChangesAsync();
        }
    }
}
