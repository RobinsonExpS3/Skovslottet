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

            var departmentTasks = new List<DepartmentTask>
            {
                new DepartmentTask { DepartmentTaskID = Guid.NewGuid(), DepartmentTaskName = "Borger kalender"},
                new DepartmentTask { DepartmentTaskID = Guid.NewGuid(), DepartmentTaskName = "FMK"},
                new DepartmentTask { DepartmentTaskID = Guid.NewGuid(), DepartmentTaskName = "Sundhedsplaner"}

            };

            context.DepartmentTasks.AddRange(departmentTasks);
            await context.SaveChangesAsync();

            return departmentTasks;
        }
    }
}
