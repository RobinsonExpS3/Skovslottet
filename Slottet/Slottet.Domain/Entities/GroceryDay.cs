namespace Slottet.Domain.Entities
{
    public class GroceryDay
    {
        public Guid GroceryDayID { get; set; }
        public string GroceryDayName { get; set; }

        public ICollection<Resident> Residents { get; set; }
    }
}
