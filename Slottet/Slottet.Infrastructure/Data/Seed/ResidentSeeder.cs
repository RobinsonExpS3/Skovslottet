using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Seed
{
    public static class ResidentSeeder
    {
        public static async Task<List<Resident>> SeedAsync(SlottetDBContext context)
        {
            if (await context.Residents.AnyAsync()) return await context.Residents.ToListAsync();

            var dayNames = new[] { "Mandag", "Tirsdag", "Onsdag", "Torsdag", "Fredag", "Lørdag", "Søndag" };

            var groceryDayIDs = await context.GroceryDays
                .Where(g => dayNames.Contains(g.GroceryDayName))
                .ToDictionaryAsync(g => g.GroceryDayName, g => g.GroceryDayID);

            Guid GetDayID(string dayName) => groceryDayIDs.TryGetValue(dayName, out var id)
                ? id
                : throw new InvalidOperationException("Dag ikke fundet");

            var residents = new List<Resident> {
                new Resident { ResidentID = Guid.NewGuid(), ResidentName = "Anna Bentsen", IsActive = true, GroceryDayID = GetDayID("Mandag") },
                new Resident { ResidentID = Guid.NewGuid(), ResidentName = "Carsten Didriksen", IsActive = true, GroceryDayID = GetDayID("Mandag") },
                new Resident { ResidentID = Guid.NewGuid(), ResidentName = "Enaya Frederiksen", IsActive = true, GroceryDayID = GetDayID("Onsdag") },
                new Resident { ResidentID = Guid.NewGuid(), ResidentName = "Gert Heller", IsActive = true, GroceryDayID = GetDayID("Fredag") },
                new Resident { ResidentID = Guid.NewGuid(), ResidentName = "Ida Jacoby", IsActive = true, GroceryDayID = GetDayID("Lørdag") },
                new Resident { ResidentID = Guid.NewGuid(), ResidentName = "Karl Larsen", IsActive = true, GroceryDayID = GetDayID("Lørdag") },
                new Resident { ResidentID = Guid.NewGuid(), ResidentName = "Mette Nielsen", IsActive = true, GroceryDayID = GetDayID("Lørdag") },
                new Resident { ResidentID = Guid.NewGuid(), ResidentName = "Ole Pontoppidan", IsActive = true, GroceryDayID = GetDayID("Lørdag") },
                new Resident { ResidentID = Guid.NewGuid(), ResidentName = "Quint Roberts", IsActive = true, GroceryDayID = GetDayID("Lørdag") },
                new Resident { ResidentID = Guid.NewGuid(), ResidentName = "Søren Thomasson", IsActive = true, GroceryDayID = GetDayID("Lørdag") },
                new Resident { ResidentID = Guid.NewGuid(), ResidentName = "Ulke Venja", IsActive = true, GroceryDayID = GetDayID("Lørdag") },
                new Resident { ResidentID = Guid.NewGuid(), ResidentName = "Whilmer Xander", IsActive = true, GroceryDayID = GetDayID("Lørdag") }
            };

            context.Residents.AddRange(residents);
            await context.SaveChangesAsync();
            return residents;
        }
    }
}
