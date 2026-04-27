using System;
using System.Collections.Generic;
using System.Text;
using Slottet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Slottet.Infrastructure.Data.Configurations
{
    public class StaffPhoneConfiguration : IEntityTypeConfiguration<StaffPhone>
    {
        public void Configure (EntityTypeBuilder<StaffPhone> entity)
        {
            entity.HasKey(sp => new { sp.StaffID, sp.PhoneID, sp.AssignedAt});

            entity.HasOne(s => s.Staff)
                .WithMany(sp => sp.StaffPhones)
                .HasForeignKey(s => s.StaffID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.Phone)
                .WithMany(sp => sp.StaffPhones)
                .HasForeignKey(p => p.PhoneID)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
