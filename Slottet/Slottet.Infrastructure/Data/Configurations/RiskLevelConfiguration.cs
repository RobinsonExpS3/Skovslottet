using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Configurations
{
    public class RiskLevelConfiguration : IEntityTypeConfiguration<RiskLevel>
    {
        public void Configure(EntityTypeBuilder<RiskLevel> entity)
        {
            throw new NotImplementedException();
        }
    }
}
