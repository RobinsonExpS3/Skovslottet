using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Seed
{
    public static class DepartmentTaskSeeder
    {
        public static async Task<List<DepartmentTask>> SeedAsync(SlottetDBContext context)
        {
            if (await context.DepartmentTasks.AnyAsync())
                return await context.DepartmentTasks.ToListAsync();

            var skoven = await context.Departments.FirstOrDefaultAsync(d => d.DepartmentName == "Skoven");
            if (skoven is null)
                throw new InvalidOperationException("Department 'Skoven' was not found.");

            var departmentTasks = new List<DepartmentTask>
            {
                new DepartmentTask { DepartmentTaskID = skoven.DepartmentID, DepartmentTaskName = "Borger kalender"},
                new DepartmentTask { DepartmentTaskID = skoven.DepartmentID, DepartmentTaskName = "FMK"},
                new DepartmentTask { DepartmentTaskID = skoven.DepartmentID, DepartmentTaskName = "Sundhedsplaner"}

            };

            context.DepartmentTasks.AddRange(departmentTasks);
            await context.SaveChangesAsync();

            return departmentTasks;
        }
    }
}
