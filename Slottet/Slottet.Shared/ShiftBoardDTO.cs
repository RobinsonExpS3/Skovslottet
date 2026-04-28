namespace Slottet.Shared
{
    public sealed class ShiftBoardDTO
    {
        public Guid ShiftBoardId { get; set; }
        public string ShiftType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string DepartmentName { get; set; } = string.Empty;

        public List<ResidentCardDto> ResidentCards { get; set; } = new();
        public List<SpecialResponsibilityEntryDto> SpecialResponsibilities { get; set; } = new();
        public List<PhoneEntryDto> PhoneNumbers { get; set; } = new();
        public List<string> DepartmentTasks { get; set; } = new();
        public List<string> AllStaff { get; set; } = new();
    }
}
