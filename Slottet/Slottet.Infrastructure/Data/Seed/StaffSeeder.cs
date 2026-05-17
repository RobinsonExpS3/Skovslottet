using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Seed
{
    public static class StaffSeeder
    {
        public static async Task<List<Staff>> SeedAsync(SlottetDBContext context)
        {
            if (await context.Staffs.AnyAsync())
            {
                return await context.Staffs.ToListAsync();
            }

            var departments = await context.Departments.ToListAsync();
            if (departments.Count == 0)
            {
                throw new InvalidOperationException("Departments must be seeded before seeding staff.");
            }

            var skovenDepartmentId = departments[0].DepartmentID;
            var slottetDepartmentId = departments.Count > 1
                ? departments[1].DepartmentID
                : skovenDepartmentId;

            var staffs = new List<Staff>
            {
                // Dev-user staff — matcher DevAuthHandler så audit-scope + StaffPN
                // har en gyldig FK at pege på. Kun til lokal udvikling.
                new Staff { StaffID = Guid.Parse("00000000-0000-0000-0000-000000000001"), StaffName = "Dev User", DepartmentID = slottetDepartmentId, Initials = "DU", Role = "admin" },
                new Staff { StaffID = Guid.NewGuid(), StaffName = "Hestemand Hestesen", DepartmentID = skovenDepartmentId, Initials = "HH", Role = "staff" },
                new Staff { StaffID = Guid.NewGuid(), StaffName = "Søren Skovfis", DepartmentID = skovenDepartmentId, Initials = "SS", Role = "staff" },
                new Staff { StaffID = Guid.NewGuid(), StaffName = "Lise Lægemand", DepartmentID = slottetDepartmentId, Initials = "LL", Role = "admin" },
                new Staff { StaffID = Guid.NewGuid(), StaffName = "Peter Pedalskid", DepartmentID = slottetDepartmentId, Initials = "PP", Role = "staff" },
            };

            context.Staffs.AddRange(staffs);
            await context.SaveChangesAsync();

            return staffs;
        }
    }
}
