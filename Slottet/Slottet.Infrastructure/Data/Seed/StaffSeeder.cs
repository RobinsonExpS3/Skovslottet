using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Seed
{
    public static class StaffSeeder
    {
        public static async Task<List<Staff>> SeedAsync(SlottetDBContext context)
        {
            if (await context.Staffs.AnyAsync())
                return await context.Staffs.ToListAsync();

            var skoven = await context.Departments.FirstOrDefaultAsync(d => d.DepartmentName == "Skoven");

            if (skoven is null)
                throw new InvalidOperationException("Department 'Skoven' was not found.");

            var staffs = new List<Staff>
            {
                new Staff { StaffID = Guid.NewGuid(), StaffName = "Gunhild Johnsen", Initials = "GJ", Role = "Administrator", DepartmentID = skoven.DepartmentID },
                new Staff { StaffID = Guid.NewGuid(), StaffName = "Georg Carstensen", Initials = "GC", Role = "Pædagog", DepartmentID = skoven.DepartmentID },
                new Staff { StaffID = Guid.NewGuid(), StaffName = "Ivan Isaksen", Initials = "II", Role = "Pædagog", DepartmentID = skoven.DepartmentID },
                new Staff { StaffID = Guid.NewGuid(), StaffName = "Inga Beck", Initials = "IB", Role = "Pædagog", DepartmentID = skoven.DepartmentID },
                new Staff { StaffID = Guid.NewGuid(), StaffName = "Albert Svendsen", Initials = "AS", Role = "Pædagog", DepartmentID = skoven.DepartmentID },
                new Staff { StaffID = Guid.NewGuid(), StaffName = "Merethe Thorsen", Initials = "MT", Role = "Pædagog", DepartmentID = skoven.DepartmentID },
                new Staff { StaffID = Guid.NewGuid(), StaffName = "Maja Wolff", Initials = "MW", Role = "Pædagog", DepartmentID = skoven.DepartmentID },
                new Staff { StaffID = Guid.NewGuid(), StaffName = "Kamilla Lang", Initials = "KL", Role = "Pædagog", DepartmentID = skoven.DepartmentID },
                new Staff { StaffID = Guid.NewGuid(), StaffName = "Anker Skov", Initials = "ASK", Role = "Administrator", DepartmentID = skoven.DepartmentID },
            };

            context.Staffs.AddRange(staffs);
            await context.SaveChangesAsync();
            return staffs;
        }
    }
}
