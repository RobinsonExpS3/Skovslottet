using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations
{
    public class SpecialResponsibilityConfiguration : IEntityTypeConfiguration<SpecialResponsibility>
    {
        /// <summary>
        /// Configures the Entity Framework model for the SpecialResponsibility entity.
        /// </summary>
        /// <param name="entity">The builder used to configure the SpecialResponsibility entity.</param>
        public void Configure(EntityTypeBuilder<SpecialResponsibility> entity)
        {
            entity.HasKey(sr => sr.SpecialResponsibilityID);

            entity.Property(sr => sr.TaskName)
                .IsRequired()
                .HasMaxLength(100);

        }
    }
}
