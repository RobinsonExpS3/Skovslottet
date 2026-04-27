using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Seed
{
    public static class GroceryDaySeeder
    {
        public static async Task<List<GroceryDay>> SeedAsync(SlottetDBContext context) {
            if(await context.GroceryDays.AnyAsync()) return await context.GroceryDays.ToListAsync();

            var groceryDays = new List<GroceryDay> {
                new GroceryDay { GroceryDayID = Guid.NewGuid(), GroceryDayName = "Mandag"},
                new GroceryDay { GroceryDayID = Guid.NewGuid(), GroceryDayName = "Tirsdag"},
                new GroceryDay { GroceryDayID = Guid.NewGuid(), GroceryDayName = "Onsdag"},
                new GroceryDay { GroceryDayID = Guid.NewGuid(), GroceryDayName = "Torsdag"},
                new GroceryDay { GroceryDayID = Guid.NewGuid(), GroceryDayName = "Fredag"},
                new GroceryDay { GroceryDayID = Guid.NewGuid(), GroceryDayName = "Lørdag"},
                new GroceryDay { GroceryDayID = Guid.NewGuid(), GroceryDayName = "Søndag"}
            };

            context.GroceryDays.AddRange(groceryDays);
            await context.SaveChangesAsync();
            return groceryDays;
        }
    }
}