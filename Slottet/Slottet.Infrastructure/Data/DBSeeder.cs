using Slottet.Infrastructure.Data.Seed;

namespace Slottet.Infrastructure.Data
{
    public static class DBSeeder
    {
        public static async Task SeedAsync(SlottetDBContext context)
        {

            // UNCOMMENT WHEN YOU HAVE FINISHED THE SEEDERS TO AVOID CONFLICTS 
            // Choose seeds, that don't have FK, or make sure to also implement relations. 



            await DepartmentSeeder.SeedAsync(context);
            //await MedicineSeeder.SeedAsync(context);
            await PaymentMethodSeeder.SeedAsync(context);
            //await ResidentStatusSeeder.SeedAsync(context);

            await context.SaveChangesAsync();

            await GroceryDaySeeder.SeedAsync(context);
            await ResidentSeeder.SeedAsync(context);
            await StaffSeeder.SeedAsync(context); //Megan
            await ShiftBoardSeeder.SeedAsync(context);

            await context.SaveChangesAsync();

            await StaffShiftSeeder.SeedAsync(context); 
            await PhoneSeeder.SeedAsync(context);
            await ResidentPaymentMethodSeeder.SeedAsync(context);
            //await StaffResidentStatusSeeder.SeedAsync(context);
            //await DepartmentTaskSeeder.SeedAsync(context);
            //await PNSeeder.SeedAsync(context);//Megan
            //await RiskLevelSeeder.SeedAsync(context); 
            await SpecialResponsibilitySeeder.SeedAsync(context); //Megan - OBS! Placering relationelt ift. shiftboard og department

            await context.SaveChangesAsync();
        }
    }
}
