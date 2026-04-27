namespace Slottet.Shared
{
    public class EditStaffDTO
    {
        public Guid StaffID { get; set; }
        public string StaffName { get; set; }
        public string Initials { get; set; }
        public string Role { get; set; }

        public Guid DepartmentID { get; set; }
    }
}