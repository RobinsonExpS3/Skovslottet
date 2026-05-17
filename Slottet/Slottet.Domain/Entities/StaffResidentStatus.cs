namespace Slottet.Domain.Entities
{
    public class StaffResidentStatus
    {
        public Guid StaffID { get; set; }
        public Guid ResidentStatusID { get; set; }
        public Guid ShiftBoardID { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.Now;

        public Staff Staff { get; set; }
        public ResidentStatus ResidentStatus { get; set; }
        public ShiftBoard ShiftBoard { get; set; }
    }
}
