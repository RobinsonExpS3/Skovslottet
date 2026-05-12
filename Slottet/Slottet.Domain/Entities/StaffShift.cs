namespace Slottet.Domain.Entities
{
    public class StaffShift
    {
        public Guid StaffShiftID { get; set; } = Guid.NewGuid();
        public Guid ShiftBoardID { get; set; }
        public ShiftBoard ShiftBoard { get; set; }

        public Guid? StaffID { get; set; }
        public Staff? Staff { get; set; }
        public bool IsDeleted { get; set; }
    }
}
