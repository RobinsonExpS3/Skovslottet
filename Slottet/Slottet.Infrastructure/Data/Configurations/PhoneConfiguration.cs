using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations {
    public class PhoneConfiguration : IEntityTypeConfiguration<Phone> {
        /// <summary>
        /// Configures the Entity Framework model for the Phone entity.
        /// </summary>
        /// <param name="entity">The builder used to configure the Phone entity.</param>
        public void Configure(EntityTypeBuilder<Phone> entity) {
            entity.HasKey(p => new { p.PhoneID });

            entity.Property(p => p.PhoneNumber)
                .IsRequired();

            entity.HasOne(d => d.Department)
                .WithMany(p => p.Phones)
                .HasForeignKey(d => d.DepartmentID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
