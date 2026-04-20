namespace Slottet.Domain.Entities
{
    public class PN
    {
        public Guid PNID { get; set; }
        public DateTime PNGivenTime { get; set; }
        public string PNStatus { get; set; }

        public Guid ResidentID { get; set; }
        public Resident Resident { get; set; }
    }
}