namespace Slottet.Domain.Entities
{
    public class Staff
    {
        public Guid StaffID { get; set; }
        public string StaffName { get; set; }
        public string Initials { get; set; }
        public string Role { get; set; }

        public Guid DepartmentID { get; set; }
        public Department Department { get; set; }

        public ICollection<StaffShift> StaffShifts { get; set; }
        public ICollection<StaffResidentStatus> StaffResidentStatuses { get; set; }

        public ICollection<StaffPhone> StaffPhones { get; set; } = new List<StaffPhone>();
        public ICollection<StaffPN> StaffPNs { get; set; } = new List<StaffPN>();

    }
}
