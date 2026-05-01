using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;


namespace Slottet.Infrastructure.Data.Seed
{
    public class StaffPNSeeder
    {
        public static async Task SeedAsync(SlottetDBContext context)
        {
            if (await context.StaffPNs.AnyAsync())
                return;
            var staff = await context.Staffs.ToListAsync();
            var pns = await context.PNs
                .OrderBy(p => p.PNReason)
                .ToListAsync();
            if (staff.Count == 0 || pns.Count == 0)
                throw new InvalidOperationException("Staff and PN must be seeded first.");
            var staffList = staff
                .OrderBy(s => s.StaffName)
                .Take(2)
                .ToList();
            var links = new List<StaffPN>();
            for (int i = 0; i < pns.Count; i++)
            {
                links.Add(new StaffPN
                {
                    StaffID = staffList[i % staffList.Count].StaffID,
                    PNID = pns[i].PNID,
                });
            }
            context.StaffPNs.AddRange(links);
            await context.SaveChangesAsync();
        }
    }
}
