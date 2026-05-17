using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations {
    public class MedicineConfiguration : IEntityTypeConfiguration<Medicine> {
        /// <summary>
        /// Configures the Entity Framework model for the Medicine entity.
        /// </summary>
        /// <param name="entity">The builder used to configure the Medicine entity.</param>
        public void Configure(EntityTypeBuilder<Medicine> entity) {
            entity.HasKey(m => m.MedicineID);

            entity.Property(m => m.ScheduledTime)
                .IsRequired()
                .HasColumnType("time");

            entity.Property(m => m.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Inaktive medicin-rækker er som standard skjult. Brug .IgnoreQueryFilters() for at se historik.
            entity.HasQueryFilter(m => m.IsActive);

            entity.HasOne(m => m.Resident)
                .WithMany(r => r.Medicines)
                .HasForeignKey(m => m.ResidentID)
                .OnDelete(DeleteBehavior.Restrict);

            // Restrict (ikke Cascade) — MedicineLogs må ikke forsvinde med Medicine.
            entity.HasMany(m => m.MedicineLogs)
                .WithOne(ml => ml.Medicine)
                .HasForeignKey(ml => ml.MedicineID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
