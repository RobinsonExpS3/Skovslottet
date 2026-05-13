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

            var today = DateOnly.FromDateTime(DateTime.Today);
            var now   = DateTime.Now;

            // (residentName, scheduledHour, scheduledMinute, givenToday)
            var data = new (string Name, int Hour, int Minute, bool Given)[]
            {
                ("Anna Bentsen",       8,  0,  true ),
                ("Anna Bentsen",      13,  0,  false),
                ("Anna Bentsen",      18,  0,  false),
                ("Anna Bentsen",      23,  0,  false),

                ("Carsten Didriksen",  8,  0,  true ),
                ("Carsten Didriksen", 13,  0,  false),
                ("Carsten Didriksen", 18,  0,  false),
                ("Carsten Didriksen", 21,  0,  false),
                ("Carsten Didriksen", 23,  0,  false),

                ("Enaya Frederiksen",  8,  0,  false),
                ("Enaya Frederiksen", 13,  0,  true ),
                ("Enaya Frederiksen", 23,  0,  true ),

                ("Gert Heller",        8,  0,  true ),
                ("Gert Heller",       23,  0,  false),

                ("Karl Larsen",        8,  0,  false),
                ("Karl Larsen",       13,  0,  true ),
                ("Karl Larsen",       18,  0,  false),

                ("Mette Nielsen",      8,  0,  true ),
                ("Mette Nielsen",     13,  0,  false),
                ("Mette Nielsen",     21,  0,  false),
                ("Mette Nielsen",     23,  0,  false),

                ("Ole Pontoppidan",    8,  0,  false),
                ("Ole Pontoppidan",   23,  0,  true ),

                ("Quint Roberts",      8,  0,  true ),
                ("Quint Roberts",     13,  0,  false),
                ("Quint Roberts",     23,  0,  false),

                ("Søren Thomasson",    8,  0,  false),
                ("Søren Thomasson",   23,  0,  false),

                ("Ulke Venja",         8,  0,  true ),
                ("Ulke Venja",        13,  0,  true ),
                ("Ulke Venja",        18,  0,  false),
                ("Ulke Venja",        21,  0,  false),
                ("Ulke Venja",        23,  0,  false),

                ("Whilmer Xander",     8,  0,  false),
                ("Whilmer Xander",    13,  0,  true ),
                ("Whilmer Xander",    23,  0,  false),
            };

            var medicines    = new List<Medicine>();
            var medicineLogs = new List<MedicineLog>();

            foreach (var (name, hour, minute, given) in data)
            {
                var resident = residents.FirstOrDefault(r => r.ResidentName == name);
                if (resident is null) continue;

                var medicine = new Medicine
                {
                    MedicineID    = Guid.NewGuid(),
                    ResidentID    = resident.ResidentID,
                    ScheduledTime = new TimeOnly(hour, minute),
                };

                medicines.Add(medicine);

                // Seed en log-post for dags dato så kortvisningen viser korrekt status.
                medicineLogs.Add(new MedicineLog
                {
                    MedicineLogID  = Guid.NewGuid(),
                    MedicineID     = medicine.MedicineID,
                    Date           = today,
                    GivenTime      = given ? DateTime.Today.AddHours(hour).AddMinutes(5) : null,
                    RegisteredTime = given ? now : null,
                });
            }

            context.Medicines.AddRange(medicines);
            context.MedicineLogs.AddRange(medicineLogs);
            await context.SaveChangesAsync();
        }
    }
}
