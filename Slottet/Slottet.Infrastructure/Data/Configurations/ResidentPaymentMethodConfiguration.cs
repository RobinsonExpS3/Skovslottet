using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations
{
    public class ResidentPaymentMethodConfiguration : IEntityTypeConfiguration<ResidentPaymentMethod>
    {
        /// <summary>
        /// Configures the Entity Framework model for the ResidentPaymentMethod entity.
        /// </summary>
        /// <param name="entity">The builder used to configure the ResidentPaymentMethod entity.</param>
        public void Configure(EntityTypeBuilder<ResidentPaymentMethod> entity)
        {
            entity.HasKey(rpm => new { rpm.ResidentID, rpm.PaymentMethodID });

            entity.HasOne(r => r.Resident)
                .WithMany(rpm => rpm.ResidentPaymentMethods)
                .HasForeignKey(r => r.ResidentID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(pm => pm.PaymentMethod)
                .WithMany(rpm => rpm.ResidentPaymentMethods)
                .HasForeignKey(pm => pm.PaymentMethodID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
