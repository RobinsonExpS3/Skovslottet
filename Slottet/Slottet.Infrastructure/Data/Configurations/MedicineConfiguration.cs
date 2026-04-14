using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations {
    public class MedicineConfiguration : IEntityTypeConfiguration<Medicine> {
        public void Configure(EntityTypeBuilder<Medicine> entity) {
            entity.HasKey(m => new { m.MedicineID });

            entity.Property(m => m.MedicineTime)
                .IsRequired();

            entity.Property(m => m.MedicineGivenTime)
                .IsRequired();

            entity.Property(m => m.MedicineRegisteredTime)
                .IsRequired();

            entity.HasOne(r => r.Resident)
                .WithMany(m => m.Medicines)
                .HasForeignKey(r => r.MedicineID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
