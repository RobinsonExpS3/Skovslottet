using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations {
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog> {
        public void Configure(EntityTypeBuilder<AuditLog> entity) {
            entity.HasKey(a => a.AuditLogID);

            entity.Property(a => a.Action)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(a => a.TableName)
                .HasMaxLength(128)
                .IsRequired();

            entity.Property(a => a.PerformedByStaffName)
                .HasMaxLength(255);
        }
    }
}
