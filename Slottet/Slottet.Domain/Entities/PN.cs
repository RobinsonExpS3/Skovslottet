namespace Slottet.Domain.Entities
{
    public class PN
    {
        public Guid PNID { get; set; }
        public DateTime PNGivenTime { get; set; }
        public string PNReason { get; set; }
        public DateTime PNRegisteredTime { get; set; }

        public Guid ResidentID { get; set; }
        public Resident Resident { get; set; }

        public ICollection<StaffPN> StaffPNs { get; set; } = new List<StaffPN>();

    }
}