using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations
{
    public class PNConfiguration : IEntityTypeConfiguration<PN>
    {
        public void Configure(EntityTypeBuilder<PN> entity)
        {
            entity.HasKey(p => p.PNID);

            entity.Property(p => p.PNGivenTime)
                .IsRequired();

            entity.Property(p => p.PNReason)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasOne(p => p.Resident)
                .WithMany(rs => rs.PNs)
                .HasForeignKey(p => p.ResidentID)
                .OnDelete(DeleteBehavior.Restrict);

        }

    }
}
