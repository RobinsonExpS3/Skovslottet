namespace Slottet.Domain.Entities
{
    public class Resident
    {
        public Guid ResidentID { get; set; }
        public string ResidentName { get; set; }
        public bool IsActive { get; set; }


        public Guid GroceryDayID { get; set; }
        public GroceryDay GroceryDay { get; set; } = null!;

        public ICollection<Medicine> Medicines { get; set; }
        public ICollection<ResidentPaymentMethod> ResidentPaymentMethods { get; set; }
        public ICollection<ResidentStatus> ResidentStatuses { get; set; }

    }
}
