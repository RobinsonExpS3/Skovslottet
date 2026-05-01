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

            var residents = await context.Residents
                .OrderBy(r => r.ResidentName)
                .ToListAsync();
            var riskLevels = await context.RiskLevels.ToListAsync();

            if (residents.Count == 0)
                throw new InvalidOperationException("Residents must be seeded before ResidentStatuses.");
            if (riskLevels.Count == 0)
                throw new InvalidOperationException("RiskLevels must be seeded before ResidentStatuses.");

            var statuses = new[]
            {
                "Har sovet godt og taget morgenmedicin uden problemer.",
                "Rolig formiddag. Har spist og været ude at gå.",
                "Virker nedtrykt og ønsker ro i dag.",
                "Har brug for ekstra støtte og tydelig guidning.",
                "Har været urolig i løbet af natten.",
                "Vil gerne have faste rutiner og korte beskeder.",
                "Følsom over for støj i fællesarealerne.",
                "Har brug for opmuntring før aktiviteter.",
                "Har været afventende i kontakt med personale.",
                "Har haft behov for hyppige pauser.",
                "Har været træt og ønsket at blive på værelset.",
                "Ønsker rolig kontakt og én medarbejder ad gangen.",
            };

            var risks = new[]
            {
                "Grøn",
                "Grøn",
                "Gul",
                "Gul",
                "Rød",
                "Rød",
                "Gul",
                "Gul",
                "Gul",
                "Rød",
                "Rød",
                "Grøn",
            };

            if (residents.Count != statuses.Length || statuses.Length != risks.Length)
                throw new InvalidOperationException("ResidentStatuses seed data must match the number of residents.");

            Guid Risk(string name) => riskLevels
                .First(r => r.RiskLevelName == name)
                .RiskLevelID;

            var residentStatuses = new List<ResidentStatus>();
            for (int i = 0; i < residents.Count; i++)
            {
                residentStatuses.Add(new ResidentStatus
                {
                    ResidentStatusID = Guid.NewGuid(),
                    ResidentID = residents[i].ResidentID,
                    RiskLevelID = Risk(risks[i]),
                    Status = statuses[i],
                    Date = DateTime.Today,
                });
            }

            context.ResidentStatuses.AddRange(residentStatuses);
            await context.SaveChangesAsync();
            return residentStatuses;
        }
    }
}
