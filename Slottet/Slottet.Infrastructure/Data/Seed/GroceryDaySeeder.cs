using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Seed
{
    public static class GroceryDaySeeder
    {
        public static async Task<List<GroceryDay>> SeedAsync(SlottetDBContext context)
        {
            if (await context.GroceryDays.AnyAsync())
                return await context.GroceryDays.ToListAsync();

            var days = new List<GroceryDay>
            {
                new() { GroceryDayID = Guid.NewGuid(), GroceryDayName = "Mandag"       },
                new() { GroceryDayID = Guid.NewGuid(), GroceryDayName = "Tirsdag"      },
                new() { GroceryDayID = Guid.NewGuid(), GroceryDayName = "Onsdag"       },
                new() { GroceryDayID = Guid.NewGuid(), GroceryDayName = "Torsdag"      },
                new() { GroceryDayID = Guid.NewGuid(), GroceryDayName = "Fredag"       },
                new() { GroceryDayID = Guid.NewGuid(), GroceryDayName = "Lørdag"       },
                new() { GroceryDayID = Guid.NewGuid(), GroceryDayName = "Søndag"       },
                new() { GroceryDayID = Guid.NewGuid(), GroceryDayName = "Handler selv" },
            };

            context.GroceryDays.AddRange(days);
            await context.SaveChangesAsync();
            return days;
        }
    }
}
