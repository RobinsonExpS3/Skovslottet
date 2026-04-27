using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Seed
{
    public static class DepartmentTaskSeeder
    {

            context.DepartmentTasks.AddRange(departmentTasks);
            await context.SaveChangesAsync();

            return departmentTasks;
        }
    }
}
