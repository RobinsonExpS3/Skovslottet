namespace Slottet.Shared
{
    public sealed class PNEntryDto
    {
        public string TimeOfAdministration { get; set; } = string.Empty;
        public string Medication { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string IssuedBy { get; set; } = "Logged-in user";
    }
}
