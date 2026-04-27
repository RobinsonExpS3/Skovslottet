using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Seed
{
    public static class ResidentStatusSeeder
    {
        public static async Task<List<ResidentStatus>> SeedAsync(SlottetDBContext context)
        {
            if (await context.ResidentStatuses.AnyAsync())
                return await context.ResidentStatuses.ToListAsync();

            var residents   = await context.Residents.ToListAsync();
            var riskLevels  = await context.RiskLevels.ToListAsync();

            if (residents.Count == 0)
                throw new InvalidOperationException("Residents must be seeded before ResidentStatuses.");
            if (riskLevels.Count == 0)
                throw new InvalidOperationException("RiskLevels must be seeded before ResidentStatuses.");

            Guid Risk(string name) => riskLevels.First(r => r.RiskLevelName == name).RiskLevelID;

            var data = new (string Name, string Note, string Risk)[]
            {
                ("Anna Bentsen",      "Har sovet godt og taget morgenmedicin uden problemer.", "Grøn"),
                ("Carsten Didriksen", "Rolig formiddag. Har spist og været ude at gå.",        "Grøn"),
                ("Enaya Frederiksen", "Virker nedtrykt og ønsker ro i dag.",                   "Gul" ),
                ("Gert Heller",       "Har brug for ekstra støtte og tydelig guidning.",        "Gul" ),
                ("Ida Jacoby",        "Har været urolig i løbet af natten.",                    "Rød" ),
                ("Karl Larsen",       "Vil gerne have faste rutiner og korte beskeder.",        "Rød" ),
                ("Mette Nielsen",     "Følsom over for støj i fællesarealerne.",                "Gul" ),
                ("Ole Pontoppidan",   "Har brug for opmuntring før aktiviteter.",               "Gul" ),
                ("Quint Roberts",     "Har været afventende i kontakt med personale.",          "Gul" ),
                ("Søren Thomasson",   "Har haft behov for hyppige pauser.",                     "Rød" ),
                ("Ulke Venja",        "Har været træt og ønsket at blive på værelset.",         "Rød" ),
                ("Whilmer Xander",    "Ønsker rolig kontakt og én medarbejder ad gangen.",      "Grøn"),
            };

            var statuses = new List<ResidentStatus>();
            foreach (var (name, note, risk) in data)
            {
                var resident = residents.FirstOrDefault(r => r.ResidentName == name);
                if (resident is null) continue;

                statuses.Add(new ResidentStatus
                {
                    ResidentStatusID = Guid.NewGuid(),
                    ResidentID       = resident.ResidentID,
                    RiskLevelID      = Risk(risk),
                    Status           = note,
                    Date             = DateTime.Today,
                });
            }

            context.ResidentStatuses.AddRange(statuses);
            await context.SaveChangesAsync();
            return statuses;
        }
    }
}
