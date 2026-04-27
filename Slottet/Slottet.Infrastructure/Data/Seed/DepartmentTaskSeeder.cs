using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Seed
{
    public static class DepartmentTaskSeeder
    {
        public static async Task SeedAsync(SlottetDBContext context)
        {
            if (await context.DepartmentTasks.AnyAsync())
                return;

            var skoven = await context.Departments
                .FirstOrDefaultAsync(d => d.DepartmentName == "Skoven");

            if (skoven is null)
                throw new InvalidOperationException("Departments must be seeded before DepartmentTasks.");

            var tasks = new[]
            {
                "Tjek borger kalender",
                "Tjekke FMK, Sundhedsplaner og Delmål løbende under hvert overlap",
                "Huske madplaner",
                "Fællesindkøb",
            };

            context.DepartmentTasks.AddRange(tasks.Select(t => new DepartmentTask
            {
                DepartmentTaskID   = Guid.NewGuid(),
                DepartmentTaskName = t,
                DepartmentID       = skoven.DepartmentID,
            }));

            await context.SaveChangesAsync();
        }
    }
}
