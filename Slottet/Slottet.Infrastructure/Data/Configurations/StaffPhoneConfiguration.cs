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
        /// <summary>
        /// Configures the Entity Framework model for the StaffPhone entity.
        /// </summary>
        /// <param name="entity">The builder used to configure the StaffPhone entity.</param>
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
