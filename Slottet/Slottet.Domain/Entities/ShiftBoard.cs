namespace Slottet.Domain.Entities
{
    public class ShiftBoard
    {
        public Guid ShiftBoardID { get; set; }
        public string ShiftType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ICollection<StaffShift> StaffShifts { get; set; }
        public ICollection<SpecialResponsibility> SpecialResponsibilities { get; set; }
    }
}