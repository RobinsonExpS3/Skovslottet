using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Seed
{
    public static class ResidentSeeder
    {
        public static async Task<List<Resident>> SeedAsync(SlottetDBContext context)
        {
            if (await context.Residents.AnyAsync())
                return await context.Residents.ToListAsync();

            var groceryDays = await context.GroceryDays.ToListAsync();
            if (groceryDays.Count == 0)
                throw new InvalidOperationException("GroceryDays must be seeded before Residents.");

            GroceryDay Day(string name) =>
                groceryDays.First(g => g.GroceryDayName == name);

            var residents = new List<Resident>
            {
                new() { ResidentID = Guid.NewGuid(), ResidentName = "Anna Bentsen",      IsActive = true, GroceryDayID = Day("Mandag").GroceryDayID  },
                new() { ResidentID = Guid.NewGuid(), ResidentName = "Carsten Didriksen", IsActive = true, GroceryDayID = Day("Mandag").GroceryDayID  },
                new() { ResidentID = Guid.NewGuid(), ResidentName = "Enaya Frederiksen", IsActive = true, GroceryDayID = Day("Onsdag").GroceryDayID  },
                new() { ResidentID = Guid.NewGuid(), ResidentName = "Gert Heller",       IsActive = true, GroceryDayID = Day("Mandag").GroceryDayID  },
                new() { ResidentID = Guid.NewGuid(), ResidentName = "Ida Jacoby",        IsActive = true, GroceryDayID = Day("Mandag").GroceryDayID  },
                new() { ResidentID = Guid.NewGuid(), ResidentName = "Karl Larsen",       IsActive = true, GroceryDayID = Day("Mandag").GroceryDayID  },
                new() { ResidentID = Guid.NewGuid(), ResidentName = "Mette Nielsen",     IsActive = true, GroceryDayID = Day("Tirsdag").GroceryDayID },
                new() { ResidentID = Guid.NewGuid(), ResidentName = "Ole Pontoppidan",   IsActive = true, GroceryDayID = Day("Torsdag").GroceryDayID },
                new() { ResidentID = Guid.NewGuid(), ResidentName = "Quint Roberts",     IsActive = true, GroceryDayID = Day("Fredag").GroceryDayID  },
                new() { ResidentID = Guid.NewGuid(), ResidentName = "Søren Thomasson",   IsActive = true, GroceryDayID = Day("Lørdag").GroceryDayID  },
                new() { ResidentID = Guid.NewGuid(), ResidentName = "Ulke Venja",        IsActive = true, GroceryDayID = Day("Søndag").GroceryDayID  },
                new() { ResidentID = Guid.NewGuid(), ResidentName = "Whilmer Xander",    IsActive = true, GroceryDayID = Day("Mandag").GroceryDayID  },
            };

            context.Residents.AddRange(residents);
            await context.SaveChangesAsync();
            return residents;
        }
    }
}
