using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations
{
    public class PNConfiguration : IEntityTypeConfiguration<PN> 
    {
        public void Configure(EntityTypeBuilder<PN> entity)
        {
            entity.HasKey(pn => new { pn.PNID });

            entity.Property(pn => pn.PNStatus)
                .IsRequired();

            entity.Property(pn => pn.PNTime)
                .IsRequired();

            entity.HasOne(rs => rs.ResidentStatus)
                .WithMany(pn => pn.PNs)
                .HasForeignKey(rs => rs.PNID)
                .OnDelete(DeleteBehavior.Restrict);

        }
        
    }
}
