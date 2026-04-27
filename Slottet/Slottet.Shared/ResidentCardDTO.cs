namespace Slottet.Shared
{
    public sealed class ResidentCardDto
    {
        public Guid     ResidentCardId { get; set; }
        public Guid     ResidentId     { get; set; }
        public DateTime Date           { get; set; }

        public string ResidentName { get; set; } = string.Empty;
        public bool   IsActive     { get; set; }

        public string?          RiskLevel        { get; set; }
        public string?          LatestStatusNote { get; set; }

        public List<MedicineScheduleItemDto> MedicineSchedule { get; set; } = new();
        public List<PNEntryDto>              PNEntries        { get; set; } = new();
        public List<string>                  AssignedStaff    { get; set; } = new();

        public string? GroceryDay     { get; set; }
        public string? PaymentMethod  { get; set; }

        public string RiskCssClass => RiskLevel switch
        {
            "Rød"  => "risk-red",
            "Gul"  => "risk-yellow",
            "Grøn" => "risk-green",
            _      => "risk-default"
        };

        public sealed class MedicineScheduleItemDto
        {
            public string   Label   { get; set; } = string.Empty;
            public TimeOnly Time    { get; set; }
            public bool     IsGiven { get; set; }
        }

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
}
