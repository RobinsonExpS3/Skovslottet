namespace Slottet.Domain.Entities
{
    public class StaffResident
    {
        public Guid StaffID { get; set; }
        public Guid ResidentID { get; set; }

        public Staff Staff { get; set; }
        public Resident Resident { get; set; }
    }
}
