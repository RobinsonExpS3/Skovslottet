namespace Slottet.Domain.Entities
{
    public class RiskLevel
    {
        public Guid RiskLevelID { get; set; }
        public string RiskLevelName { get; set; }

        public ICollection<ResidentStatus> ResidentStatuses { get; set; }
    }
}
