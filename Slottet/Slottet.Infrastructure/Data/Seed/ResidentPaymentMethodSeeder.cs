using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Seed
{
    public static class ResidentPaymentMethodSeeder
    {
        public static async Task <List<ResidentPaymentMethod>> SeedAsync(SlottetDBContext context)
        {
            if (await context.ResidentPaymentMethods.AnyAsync()) return await context.ResidentPaymentMethods.ToListAsync();

            var residentPaymentMethods = new List<ResidentPaymentMethod>
            {
                new ResidentPaymentMethod { ResidentID = Guid.NewGuid(), PaymentMethodID = Guid.NewGuid() },
                new ResidentPaymentMethod { ResidentID = Guid.NewGuid(), PaymentMethodID = Guid.NewGuid() },
                new ResidentPaymentMethod { ResidentID = Guid.NewGuid(), PaymentMethodID = Guid.NewGuid() },
                new ResidentPaymentMethod { ResidentID = Guid.NewGuid(), PaymentMethodID = Guid.NewGuid() },
                new ResidentPaymentMethod { ResidentID = Guid.NewGuid(), PaymentMethodID = Guid.NewGuid() },
                new ResidentPaymentMethod { ResidentID = Guid.NewGuid(), PaymentMethodID = Guid.NewGuid() }
            };

            context.ResidentPaymentMethods.AddRange(residentPaymentMethods);
            await context.SaveChangesAsync();
            return residentPaymentMethods;
        }
    }
}
