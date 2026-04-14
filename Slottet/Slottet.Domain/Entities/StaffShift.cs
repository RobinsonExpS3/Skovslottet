namespace Slottet.Domain.Entities
{
    public class StaffShift
    {
        public Guid ShiftBoardID { get; set; }
        //public ShiftBoard ShiftBoard { get; set; }

        public Guid StaffID { get; set; }
        //public Staff Staff { get; set; }

        public ICollection<Staff> Staffs { get; set; }
        public ICollection<ShiftBoard> ShiftBoards { get; set; }
        public ShiftBoard ShiftBoard { get; set; }
    }
}
