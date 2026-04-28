using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Seed
{
    public static class ResidentPaymentMethodSeeder
    {
        public static async Task<List<ResidentPaymentMethod>> SeedAsync(SlottetDBContext context)
        {
            if (await context.ResidentPaymentMethods.AnyAsync()) return await context.ResidentPaymentMethods.ToListAsync();

            var data = new (string Resident, string PaymentMethod)[]
            {
                ("Anna Bentsen", "MobilePay"),
                ("Carsten Didriksen", "MobilePay"),
                ("Enaya Frederiksen", "P-kort"),
                ("Gert Heller", "Udlæg"),
                ("Ida Jacoby", "Kort"),
                ("Karl Larsen", "Kontant"),
            };

            var residents = await context.Residents.ToListAsync();
            var paymentMethods = await context.PaymentMethods.ToListAsync();

            var residentPaymentMethods = new List<ResidentPaymentMethod>();
            foreach (var (residentName, paymentMethodName) in data)
            {
                var resident = residents.FirstOrDefault(r => r.ResidentName == residentName);
                var paymentMethod = paymentMethods.FirstOrDefault(p => p.PaymentMethodName == paymentMethodName);

                if (resident is null || paymentMethod is null)
                    continue;

                residentPaymentMethods.Add(new ResidentPaymentMethod
                {
                    ResidentID = resident.ResidentID,
                    PaymentMethodID = paymentMethod.PaymentMethodID,
                });
            }

            context.ResidentPaymentMethods.AddRange(residentPaymentMethods);
            await context.SaveChangesAsync();
            return residentPaymentMethods;
        }
    }
}
