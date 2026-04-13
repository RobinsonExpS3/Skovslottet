namespace Slottet.Domain.Entities
{
    public class StaffShift
    {
        public ShiftBoard ShiftBoardID { get; set; }
        public Staff StaffID { get; set; }

        public ICollection<Staff> Staff { get; set; }
        public ICollection<ShiftBoard> ShiftBoards { get; set; }
    }
}
