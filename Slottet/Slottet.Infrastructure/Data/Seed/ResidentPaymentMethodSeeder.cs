using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Seed
{
    public static class ResidentPaymentMethodSeeder
    {
        public static async Task SeedAsync(SlottetDBContext context)
        {
            if (await context.ResidentPaymentMethods.AnyAsync())
                return;

            var residents      = await context.Residents.ToListAsync();
            var paymentMethods = await context.PaymentMethods.ToListAsync();

            if (residents.Count == 0 || paymentMethods.Count == 0)
                throw new InvalidOperationException("Residents and PaymentMethods must be seeded first.");

            Guid PM(string name) => paymentMethods.First(p => p.PaymentMethodName == name).PaymentMethodID;

            var data = new (string Resident, string Payment)[]
            {
                ("Anna Bentsen",      "Kort"     ),
                ("Carsten Didriksen", "MobilePay"),
                ("Enaya Frederiksen", "Kontant"  ),
                ("Gert Heller",       "MobilePay"),
                ("Ida Jacoby",        "MobilePay"),
                ("Karl Larsen",       "MobilePay"),
                ("Mette Nielsen",     "MobilePay"),
                ("Ole Pontoppidan",   "MobilePay"),
                ("Quint Roberts",     "MobilePay"),
                ("Søren Thomasson",   "MobilePay"),
                ("Ulke Venja",        "MobilePay"),
                ("Whilmer Xander",    "MobilePay"),
            };

            var links = new List<ResidentPaymentMethod>();
            foreach (var (name, payment) in data)
            {
                var resident = residents.FirstOrDefault(r => r.ResidentName == name);
                if (resident is null) continue;

                links.Add(new ResidentPaymentMethod
                {
                    ResidentID      = resident.ResidentID,
                    PaymentMethodID = PM(payment),
                });
            }

            context.ResidentPaymentMethods.AddRange(links);
            await context.SaveChangesAsync();
        }
    }
}
