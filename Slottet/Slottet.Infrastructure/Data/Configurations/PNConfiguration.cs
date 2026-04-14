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

            entity.Property(p => p.PNTime)
                .IsRequired();

            entity.Property(p => p.PNStatus)
                .IsRequired()
                .HasMaxLength(255);

        }

    }
}
