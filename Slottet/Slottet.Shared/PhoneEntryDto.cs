namespace Slottet.Shared
{
    public sealed class PhoneEntryDto
    {
        public Guid PhoneID { get; set; }
        public string Number { get; set; } = string.Empty;
        public string StaffName { get; set; } = string.Empty;
    }
}
