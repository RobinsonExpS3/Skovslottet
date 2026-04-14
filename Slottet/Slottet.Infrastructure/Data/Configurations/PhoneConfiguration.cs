using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations {
    public class PhoneConfiguration : IEntityTypeConfiguration<Phone> {
        public void Configure(EntityTypeBuilder<Phone> entity) {
            entity.HasKey(r => new { r.PhoneID });

            entity.Property(r => r.PhoneNumber)
                .IsRequired();

            entity.HasOne(r => r.Department)
                .WithMany(g => g.Phones)
                .HasForeignKey(r => r.DepartmentID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
