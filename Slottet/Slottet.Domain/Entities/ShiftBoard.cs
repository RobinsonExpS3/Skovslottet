namespace Slottet.Domain.Entities
{
    public class ShiftBoard
    {
        public Guid ShiftBoardID { get; set; }
        public string ShiftType { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        public Guid? DepartmentID { get; set; }
        public Department Department { get; set; }
    }
}