using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Seed
{
    public static class MedicineSeeder
    {
        public static async Task SeedAsync(SlottetDBContext context)
        {
            if (await context.Medicines.AnyAsync())
                return;

            var residents = await context.Residents.ToListAsync();
            if (residents.Count == 0)
                throw new InvalidOperationException("Residents must be seeded before Medicines.");

            var today = DateTime.Today;
            var now   = DateTime.Now;

            // (residentName, scheduledHour, wasGiven)
            var data = new (string Name, int Hour, bool Given)[]
            {
                ("Anna Bentsen",       8,  true ),
                ("Anna Bentsen",      13,  false),
                ("Anna Bentsen",      18,  false),
                ("Anna Bentsen",      23,  false),

                ("Carsten Didriksen",  8,  true ),
                ("Carsten Didriksen", 13,  false),
                ("Carsten Didriksen", 18,  false),
                ("Carsten Didriksen", 21,  false),
                ("Carsten Didriksen", 23,  false),

                ("Enaya Frederiksen",  8,  false),
                ("Enaya Frederiksen", 13,  true ),
                ("Enaya Frederiksen", 23,  true ),

                ("Gert Heller",        8,  true ),
                ("Gert Heller",       23,  false),

                ("Karl Larsen",        8,  false),
                ("Karl Larsen",       13,  true ),
                ("Karl Larsen",       18,  false),

                ("Mette Nielsen",      8,  true ),
                ("Mette Nielsen",     13,  false),
                ("Mette Nielsen",     21,  false),
                ("Mette Nielsen",     23,  false),

                ("Ole Pontoppidan",    8,  false),
                ("Ole Pontoppidan",   23,  true ),

                ("Quint Roberts",      8,  true ),
                ("Quint Roberts",     13,  false),
                ("Quint Roberts",     23,  false),

                ("Søren Thomasson",    8,  false),
                ("Søren Thomasson",   23,  false),

                ("Ulke Venja",         8,  true ),
                ("Ulke Venja",        13,  true ),
                ("Ulke Venja",        18,  false),
                ("Ulke Venja",        21,  false),
                ("Ulke Venja",        23,  false),

                ("Whilmer Xander",     8,  false),
                ("Whilmer Xander",    13,  true ),
                ("Whilmer Xander",    23,  false),
            };

            var medicines = new List<Medicine>();
            foreach (var (name, hour, given) in data)
            {
                var resident = residents.FirstOrDefault(r => r.ResidentName == name);
                if (resident is null) continue;

                medicines.Add(new Medicine
                {
                    MedicineID             = Guid.NewGuid(),
                    ResidentID             = resident.ResidentID,
                    MedicineTime           = today.AddHours(hour),
                    MedicineGivenTime      = given ? today.AddHours(hour).AddMinutes(5) : default,
                    MedicineRegisteredTime = now,
                });
            }

            context.Medicines.AddRange(medicines);
            await context.SaveChangesAsync();
        }
    }
}
