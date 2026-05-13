using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations
{
    public class MedicineLogConfiguration : IEntityTypeConfiguration<MedicineLog>
    {
        /// <summary>
        /// Configures the Entity Framework model for the MedicineLog entity.
        /// </summary>
        public void Configure(EntityTypeBuilder<MedicineLog> entity)
        {
            entity.HasKey(ml => ml.MedicineLogID);

            entity.Property(ml => ml.Date)
                .IsRequired()
                .HasColumnType("date");

            entity.Property(ml => ml.GivenTime)
                .HasColumnType("datetime2");

            entity.Property(ml => ml.RegisteredTime)
                .HasColumnType("datetime2");

            // FK configured in MedicineConfiguration (Cascade)
        }
    }
}
