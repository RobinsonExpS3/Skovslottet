using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
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
