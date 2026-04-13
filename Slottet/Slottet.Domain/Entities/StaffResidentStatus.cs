namespace Slottet.Domain.Entities
{
    public class StaffResidentStatus
    {
        public Guid StaffID { get; set; }
        public Guid ResidentStatusID { get; set; }

        public Staff Staff { get; set; }
        public Resident ResidentStatus { get; set; }
    }
}
