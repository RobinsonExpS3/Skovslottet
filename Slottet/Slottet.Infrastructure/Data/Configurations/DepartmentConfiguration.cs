using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        /// <summary>
        /// Configures the Entity Framework model for the Department entity.
        /// </summary>
        /// <param name="entity">The builder used to configure the Department entity.</param>
        public void Configure(EntityTypeBuilder<Department> entity)
        {
            entity.HasKey(d => new { d.DepartmentID });

            entity.Property(d => d.DepartmentName)
                .IsRequired();

            entity.HasMany(dt => dt.DepartmentTasks)
                .WithOne(d => d.Department)
                .HasForeignKey(d => d.DepartmentID);

            entity.HasMany(p => p.Phones)
                .WithOne(d => d.Department)
                .HasForeignKey(d => d.DepartmentID);

            entity.HasMany(s => s.Staffs)
                .WithOne(d => d.Department)
                .HasForeignKey(d => d.DepartmentID);
        }
    }
}
