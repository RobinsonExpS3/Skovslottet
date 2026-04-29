using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Seed
{
    public static class RiskLevelSeeder
    {
        public static async Task<List<RiskLevel>> SeedAsync(SlottetDBContext context)
        {
            if (await context.RiskLevels.AnyAsync())
                return await context.RiskLevels.ToListAsync();

            var levels = new List<RiskLevel>
            {
                new() { RiskLevelID = Guid.NewGuid(), RiskLevelName = "Grøn" },
                new() { RiskLevelID = Guid.NewGuid(), RiskLevelName = "Gul"  },
                new() { RiskLevelID = Guid.NewGuid(), RiskLevelName = "Rød"  },
            };

            context.RiskLevels.AddRange(levels);
            await context.SaveChangesAsync();
            return levels;
        }
    }
}
