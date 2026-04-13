namespace Slottet.Domain.Entities
{
    public class Staff
    {
        public Guid StaffID { get; set; }
        public string StaffName { get; set; }
        public string Initials { get; set; }
        public string Role { get; set; }

        public Department DepartmentID { get; set; }
        public Department Department { get; set; }
    }
}
