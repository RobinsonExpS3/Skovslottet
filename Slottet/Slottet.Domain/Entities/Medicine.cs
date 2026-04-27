namespace Slottet.Domain.Entities
{
    public class Medicine
    {
        public Guid MedicineID { get; set; }
        public DateTime MedicineTime { get; set; }
        public DateTime MedicineGivenTime { get; set; }
        public DateTime MedicineRegisteredTime { get; set; }

        public Guid ResidentID { get; set; }
        public Resident Resident { get; set; }
    }
}