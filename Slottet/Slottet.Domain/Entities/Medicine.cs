namespace Slottet.Domain.Entities
{
    public class Medicine
    {
        public Medicine MedicineID { get; set; }
        public Medicine MedicineTime { get; set; }
        public Medicine MedicinGivenTime { get; set; }
        public Medicine MedicineRegisteredTime { get; set; }

        public Guid ResidentID { get; set; }
        public Resident Resident { get; set; }
    }
}
