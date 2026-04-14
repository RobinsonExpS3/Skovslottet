using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations
{
    public class DepartmentTaskConfiguration : IEntityTypeConfiguration<DepartmentTask>
    {
        public void Configure(EntityTypeBuilder<DepartmentTask> entity)
        {
            entity.HasKey(r => new { r.DepartmentTaskID });

            entity.Property(r => r.DepartmentTaskName)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(r => r.Department)
                .WithMany(g => g.DepartmentTasks)
                .HasForeignKey(r => r.DepartmentID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
