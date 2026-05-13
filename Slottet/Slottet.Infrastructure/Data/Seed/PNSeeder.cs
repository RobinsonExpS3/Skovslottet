using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Seed
{
    public static class PNSeeder
    {
        public static async Task SeedAsync(SlottetDBContext context)
        {
            if (await context.PNs.AnyAsync())
                return;

            var residents = await context.Residents.ToListAsync();
            if (residents.Count == 0)
                throw new InvalidOperationException("Residents must be seeded before PNs.");

            var today = DateTime.Today;
            var now   = DateTime.Now;

            // (residentName, medicationName, reason, hoursAgo)
            var data = new (string Name, string Medication, string Reason, double HoursAgo)[]
            {
                ("Anna Bentsen",       "Panodil 500mg",   "Smerter i ryggen",       3.5),
                ("Anna Bentsen",       "Stesolid 5mg",    "Angst / uro",            1.0),
                ("Carsten Didriksen",  "Ibuprofen 400mg", "Hovedpine",              5.0),
                ("Enaya Frederiksen",  "Morfin 10mg",     "Stærke smerter",         2.0),
                ("Gert Heller",        "Panodil 1g",      "Feber",                  4.0),
                ("Karl Larsen",        "Lorazepam 1mg",   "Søvnbesvær",             6.5),
                ("Mette Nielsen",      "Zofran 4mg",      "Kvalme efter medicin",   1.5),
                ("Ole Pontoppidan",    "Panodil 500mg",   "Ledsmerter",             7.0),
                ("Quint Roberts",      "Ibuprofen 200mg", "Tandpine",               3.0),
            };

            var pns = new List<PN>();
            foreach (var (name, medication, reason, hoursAgo) in data)
            {
                var resident = residents.FirstOrDefault(r => r.ResidentName == name);
                if (resident is null) continue;

                pns.Add(new PN
                {
                    PNID               = Guid.NewGuid(),
                    ResidentID         = resident.ResidentID,
                    MedicationName     = medication,
                    PNReason           = reason,
                    PNGivenTime        = today.AddHours(-hoursAgo),
                    PNRegisteredTime   = now,
                });
            }

            context.PNs.AddRange(pns);
            await context.SaveChangesAsync();
        }
    }
}
