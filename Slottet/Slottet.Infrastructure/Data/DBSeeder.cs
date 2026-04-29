using Slottet.Infrastructure.Data.Seed;

namespace Slottet.Infrastructure.Data
{
    public static class DBSeeder
    {
        public static async Task SeedAsync(SlottetDBContext context)
        {
            // ── Tier 1: no FKs ───────────────────────────────────────────
            await DepartmentSeeder.SeedAsync(context);
            await GroceryDaySeeder.SeedAsync(context);
            await RiskLevelSeeder.SeedAsync(context);
            await PaymentMethodSeeder.SeedAsync(context);

            // ── Tier 2: FK → Tier 1 ──────────────────────────────────────
            await StaffSeeder.SeedAsync(context);
            await ShiftBoardSeeder.SeedAsync(context);
            await ResidentSeeder.SeedAsync(context);
            await DepartmentTaskSeeder.SeedAsync(context);

            // ── Tier 3: FK → Tier 2 ──────────────────────────────────────
            await ResidentStatusSeeder.SeedAsync(context);
            await ResidentPaymentMethodSeeder.SeedAsync(context);
            await MedicineSeeder.SeedAsync(context);
            await PhoneSeeder.SeedAsync(context);
            await StaffShiftSeeder.SeedAsync(context);

            // ── Tier 4: FK → Tier 3 ──────────────────────────────────────
            await StaffResidentStatusSeeder.SeedAsync(context);
        }
    }
}
