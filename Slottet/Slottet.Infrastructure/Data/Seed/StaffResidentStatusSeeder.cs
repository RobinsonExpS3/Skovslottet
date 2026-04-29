using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Seed
{
    public static class StaffResidentStatusSeeder
    {
        public static async Task SeedAsync(SlottetDBContext context)
        {
            if (await context.StaffResidentStatuses.AnyAsync())
                return;

            var staff = await context.Staffs.ToListAsync();
            var statuses = await context.ResidentStatuses
                .OrderBy(rs => rs.Resident.ResidentName)
                .ToListAsync();

            if (staff.Count == 0 || statuses.Count == 0)
                throw new InvalidOperationException("Staff and ResidentStatuses must be seeded first.");

            var staffList = staff
                .OrderBy(s => s.StaffName)
                .Take(2)
                .ToList();

            var links = new List<StaffResidentStatus>();

            for (int i = 0; i < statuses.Count; i++)
            {
                links.Add(new StaffResidentStatus
                {
                    StaffID = staffList[i % staffList.Count].StaffID,
                    ResidentStatusID = statuses[i].ResidentStatusID,
                });
            }

            context.StaffResidentStatuses.AddRange(links);
            await context.SaveChangesAsync();
        }
    }
}
