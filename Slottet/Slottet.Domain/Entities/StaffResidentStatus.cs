namespace Slottet.Domain.Entities
{
    public class StaffResidentStatus
    {
        public Guid StaffResidentStatusID { get; set; } = Guid.NewGuid();
        public Guid? StaffID { get; set; }
        public Guid ResidentStatusID { get; set; }
        public bool IsDeleted { get; set; }

        public Staff? Staff { get; set; }
        public ResidentStatus ResidentStatus { get; set; }
    }
}
