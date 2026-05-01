using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations {
    public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod> {
        /// <summary>
        /// Configures the Entity Framework model for the PaymentMethod entity.
        /// </summary>
        /// <param name="entity">The builder used to configure the PaymentMethod entity.</param>
        public void Configure(EntityTypeBuilder<PaymentMethod> entity) {
            entity.HasKey(pm => new { pm.PaymentMethodID });

            entity.Property(pm => pm.PaymentMethodName)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
