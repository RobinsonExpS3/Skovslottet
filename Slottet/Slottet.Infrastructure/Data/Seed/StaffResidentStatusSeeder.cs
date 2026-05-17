using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Seed
{
    public static class StaffResidentStatusSeeder
    {
        public static Task SeedAsync(SlottetDBContext context)
        {
            // Staff-assignments er per-shiftboard og sættes af brugere i UI — ikke pre-seeded.
            return Task.CompletedTask;
        }
    }
}
