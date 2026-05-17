namespace Slottet.Shared
{
    public sealed class SpecialResponsibilityAssignmentDto
    {
        public Guid SpecialResponsibilityID { get; set; }
        public Guid ShiftBoardID { get; set; }
        public Guid DepartmentID { get; set; }
        public string StaffName { get; set; } = string.Empty;
    }
}
