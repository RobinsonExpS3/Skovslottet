using Microsoft.EntityFrameworkCore;
using Slottet.Infrastructure.Data;

namespace Slottet.API
{
    public static class DBStartExtensionHelper
    {
        public static async Task MigrateAndSeedDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<SlottetDBContext>();

            await context.Database.MigrateAsync();
            await DBSeeder.SeedAsync(context);
        }
    }
}
