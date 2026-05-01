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
        /// <summary>
        /// Configures the Entity Framework model for the DepartmentTask entity.
        /// </summary>
        /// <param name="entity">The builder used to configure the DepartmentTask entity.</param>
        public void Configure(EntityTypeBuilder<DepartmentTask> entity)
        {
            entity.HasKey(dt => new { dt.DepartmentTaskID });

            entity.Property(dt => dt.DepartmentTaskName)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(d => d.Department)
                .WithMany(dt => dt.DepartmentTasks)
                .HasForeignKey(d => d.DepartmentID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
