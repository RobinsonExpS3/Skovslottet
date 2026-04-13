namespace Slottet.Domain.Entities
{
    public class Resident
    {
        public Guid ResidentID { get; set; }
        public string ResidentName { get; set; }
        public bool IsActive { get; set; }


        public Guid GroceryDayID { get; set; }
        public GroceryDay GroceryDay { get; set; } = null!;

    }
}
