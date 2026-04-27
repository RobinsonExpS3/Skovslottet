namespace Slottet.Shared
{
    public sealed class PNEntryDto
    {
        public string TimeOfAdministration { get; set; } = string.Empty;
        public string Medication           { get; set; } = string.Empty;
        public string Reason               { get; set; } = string.Empty;

        /// <summary>
        /// Displayed as static text — set automatically to the logged-in user
        /// when a PN entry is created. Cannot be edited manually.
        /// </summary>
        public string IssuedBy { get; set; } = "Logged-in user";
    }
}
