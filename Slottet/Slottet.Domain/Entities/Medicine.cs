namespace Slottet.Domain.Entities
{
    public class Medicine
    {
        public Guid MedicineID { get; set; }
        public DateTime MedicineTime { get; set; }
        public DateTime MedicinGivenTime { get; set; }
        public DateTime MedicineRegisteredTime { get; set; }
    }
}
