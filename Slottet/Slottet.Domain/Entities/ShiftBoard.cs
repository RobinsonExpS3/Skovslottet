namespace Slottet.Domain.Entities
{
    public class ShiftBoard
    {
        public Guid ShiftBoardID { get; set; }
        public string ShiftType { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        public ICollection<StaffShift> StaffShifts { get; set; }
    }
}