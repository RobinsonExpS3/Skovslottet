namespace Slottet.Shared
{
    public sealed class SpecialResponsibilityEntryDto
    {
        public Guid SpecialResponsibilityID { get; set; }
        public string Description { get; set; } = string.Empty;
        public string StaffName { get; set; } = string.Empty;
        public string StaffInitials { get; set; } = string.Empty;
    }
}
