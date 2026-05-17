using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Seed
{
    public static class SpecialResponsibilitySeeder
    {
        public static async Task<List<SpecialResponsibility>> SeedAsync(SlottetDBContext context)
        {
            if (await context.SpecialResponsibilities.AnyAsync())
                return await context.SpecialResponsibilities.ToListAsync();

            var department = await context.Departments.FirstOrDefaultAsync()
                ?? throw new InvalidOperationException("Departments must be seeded before SpecialResponsibilities.");

            var specialResponsibilities = new List<SpecialResponsibility>
            {
                new SpecialResponsibility { SpecialResponsibilityID = Guid.NewGuid(), TaskName = "Omsorgsperson",       DepartmentID = department.DepartmentID },
                new SpecialResponsibility { SpecialResponsibilityID = Guid.NewGuid(), TaskName = "Aftensmad",           DepartmentID = department.DepartmentID },
                new SpecialResponsibility { SpecialResponsibilityID = Guid.NewGuid(), TaskName = "Hygiejne/afsprit",    DepartmentID = department.DepartmentID },
                new SpecialResponsibility { SpecialResponsibilityID = Guid.NewGuid(), TaskName = "Kaffe til næste hold",DepartmentID = department.DepartmentID },
                new SpecialResponsibility { SpecialResponsibilityID = Guid.NewGuid(), TaskName = "Tøm skraldespand",    DepartmentID = department.DepartmentID },
                new SpecialResponsibility { SpecialResponsibilityID = Guid.NewGuid(), TaskName = "Søndag: Madplan!",    DepartmentID = department.DepartmentID },
            };

            context.SpecialResponsibilities.AddRange(specialResponsibilities);
            await context.SaveChangesAsync();

            return specialResponsibilities;
        }
    }
}
