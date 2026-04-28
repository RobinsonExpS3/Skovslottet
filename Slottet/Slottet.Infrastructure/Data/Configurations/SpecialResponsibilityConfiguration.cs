using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations
{
    public class SpecialResponsibilityConfiguration : IEntityTypeConfiguration<SpecialResponsibility>
    {
        public void Configure(EntityTypeBuilder<SpecialResponsibility> entity)
        {
            entity.HasKey(sr => sr.SpecialResponsibilityID);

            entity.Property(sr => sr.TaskName)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(sb => sb.SpecialResponsibilityStaff)
                .WithMany(sb => sb.SpecialResponsibilities)
                .HasForeignKey(sr => sr.??????)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}