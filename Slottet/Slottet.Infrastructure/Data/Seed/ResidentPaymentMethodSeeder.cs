using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Seed
{
    public static class ResidentPaymentMethodSeeder
    {
        public static async Task <List<ResidentPaymentMethod>> SeedAsync(SlottetDBContext context)
        {
            if (await context.ResidentPaymentMethods.AnyAsync()) return await context.ResidentPaymentMethods.ToListAsync();

            var paymentMethods = new[] { "MobilePay", "P-kort", "Kontant", "Kort", "Udlæg"};

            var paymentMethodyIDs = await context.PaymentMethods
                .Where(p => paymentMethods.Contains(p.PaymentMethodName))
                .ToDictionaryAsync(p => p.PaymentMethodName, p => p.PaymentMethodID);

            Guid GetPaymentMethodID(string paymentMethod) => paymentMethodyIDs.TryGetValue(paymentMethod, out var id)
                ? id
                : throw new InvalidOperationException("Betalingsmetode ikke fundet");

            var residents = new[] { "Carl Johan", "Trøffel", "Rød Fluesvamp", "Portobello", "Kantarel" };

            var residentIDs = await context.Residents
                .Where(r => residents.Contains(r.ResidentName))
                .ToDictionaryAsync(r => r.ResidentName, r => r.ResidentID);

            Guid GetResidentID(string resident) => residentIDs.TryGetValue(resident, out var id)
                ? id
                : throw new InvalidOperationException("Beboer ikke fundet");


            var residentPaymentMethods = new List<ResidentPaymentMethod>
            {
                new ResidentPaymentMethod { ResidentID = GetResidentID("Carl Johan"), PaymentMethodID = GetPaymentMethodID("MobilePay") },
                new ResidentPaymentMethod { ResidentID = GetResidentID("Trøffel"), PaymentMethodID = GetPaymentMethodID("MobilePay") },
                new ResidentPaymentMethod { ResidentID = GetResidentID("Rød Fluesvamp"), PaymentMethodID = GetPaymentMethodID("MobilePay") },
                new ResidentPaymentMethod { ResidentID = GetResidentID("Portobello"), PaymentMethodID = GetPaymentMethodID("Udlæg") },
                new ResidentPaymentMethod { ResidentID = GetResidentID("Kantarel"), PaymentMethodID = GetPaymentMethodID("Kort") },
                new ResidentPaymentMethod { ResidentID = GetResidentID("Kantarel"), PaymentMethodID = GetPaymentMethodID("Kontant") }
            };

            context.ResidentPaymentMethods.AddRange(residentPaymentMethods);
            await context.SaveChangesAsync();
            return residentPaymentMethods;
        }
    }
}
