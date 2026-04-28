namespace Slottet.Shared
{
    public sealed class ResidentCardDto
    {
        public Guid ResidentStatusID { get; set; }
        public Guid ResidentID { get; set; }
        public DateTime Date { get; set; }

        public string ResidentName { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public string? RiskLevel { get; set; }
        public string? LatestStatusNote { get; set; }

        public List<MedicineScheduleItemDto> MedicineSchedule { get; set; } = new();
        public List<PNEntryDto> PNEntries { get; set; } = new();
        public List<string> AssignedStaff { get; set; } = new();

        public string? GroceryDay { get; set; }
        public string? PaymentMethod { get; set; }

        public string RiskCssClass => RiskLevel switch
        {
            "Rød" => "risk-red",
            "Gul" => "risk-yellow",
            "Grøn" => "risk-green",
            _ => "risk-default"
        };
    }
}
