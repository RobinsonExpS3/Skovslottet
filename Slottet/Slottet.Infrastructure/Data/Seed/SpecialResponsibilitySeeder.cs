using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Seed
{
    public static class SpecialResponsibilitySeeder
    {
        public static async Task<List<SpecialResponsibility>> SeedAsync(SlottetDBContext context)
        {
            if (await context.SpecialResponsibilities.AnyAsync())
            return await context.SpecialResponsibilities.ToListAsync();

            var specialResponsibilities = new List<SpecialResponsibility>
            {
                new SpecialResponsibility { SpecialResponsibilityID=Guid.NewGuid(), TaskName = "Medicin Tovholder", ShiftBoardID = /* Giver ikke mening og bryder med normalform*/},
                new SpecialResponsibility { SpecialResponsibilityID=Guid.NewGuid(), TaskName = "Omsorgsperson"},
                new SpecialResponsibility { SpecialResponsibilityID=Guid.NewGuid(), TaskName = "Aftensmad"},
                new SpecialResponsibility { SpecialResponsibilityID=Guid.NewGuid(), TaskName = "Hygiejne/afsprit"},
                new SpecialResponsibility { SpecialResponsibilityID=Guid.NewGuid(), TaskName = "Kaffe til næste hold"},
                new SpecialResponsibility { SpecialResponsibilityID=Guid.NewGuid(), TaskName = "Tøm skraldespand"},
                new SpecialResponsibility { SpecialResponsibilityID=Guid.NewGuid(), TaskName = "Søndag: Madplan!"}
            };

            context.SpecialResponsibilities.AddRange(specialResponsibilities);
            await context.SaveChangesAsync();

            return specialResponsibilities;
        }
    }
}
