namespace Slottet.Domain.Entities
{
    public class SpecialResponsibility
    {
        public Guid SpecialResponsibilityID { get; set; }
        public string TaskName { get; set; }
        public Guid DepartmentID { get; set; }

        public Department Department { get; set; }
        public ICollection<SpecialResponsibilityStaff> SpecialResponsibilityStaffs { get; set; }
            = new List<SpecialResponsibilityStaff>();
    }
}
