using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Seed
{
    public static class PaymentMethodSeeder
    {
        public static async Task<List<PaymentMethod>> SeedAsync(SlottetDBContext context) {
            if(await context.PaymentMethods.AnyAsync()) {
                return await context.PaymentMethods.ToListAsync();
            }

            var paymentMethods = new[] {
                "Kort",
                "Kontant",
                "MobilePay",
                "P-Kort",
                "Udlæg"
            };

            var existingPaymentMethods = await context.PaymentMethods.ToListAsync();

            var existingNames = existingPaymentMethods.Select(x => x.PaymentMethodName?.Trim() ?? string.Empty).ToList();

            var paymentMethodsToAdd = paymentMethods.Where(name => !existingNames.Contains(name)).Select(name => new PaymentMethod {
                PaymentMethodID = Guid.NewGuid(),
                PaymentMethodName = name
            });

            if (!paymentMethodsToAdd.Any()) {
                return existingPaymentMethods;
            }

            context.PaymentMethods.AddRange(paymentMethodsToAdd);
            await context.SaveChangesAsync();

            existingPaymentMethods.AddRange(paymentMethodsToAdd);
            return existingPaymentMethods;
        }
    }
}
