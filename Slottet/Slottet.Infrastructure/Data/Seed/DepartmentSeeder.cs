using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Seed
{
    public static class DepartmentSeeder
    {
        public static async Task<List<Department>> SeedAsync(SlottetDBContext context)
        {
            if (await context.Departments.AnyAsync())
                return await context.Departments.ToListAsync();

            var departments = new List<Department>
            {
                new Department { DepartmentID = Guid.NewGuid(), DepartmentName = "Skoven" },
                new Department { DepartmentID = Guid.NewGuid(), DepartmentName = "Slottet" }
            };

            context.Departments.AddRange(departments);
            await context.SaveChangesAsync();

            return departments;
        }
    }
}
