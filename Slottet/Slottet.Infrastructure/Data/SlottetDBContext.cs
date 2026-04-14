using Microsoft.EntityFrameworkCore;

namespace Slottet.Infrastructure.Data
{
    public class SlottetDBContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SlottetDBContext).Assembly);
        }
    }
}
