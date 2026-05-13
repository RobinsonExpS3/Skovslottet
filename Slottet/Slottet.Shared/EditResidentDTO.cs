namespace Slottet.Shared {
    public class EditResidentDTO {
        public Guid ResidentID { get; set; }
        public string ResidentName { get; set; } = string.Empty;

        public Guid GroceryDayID { get; set; }

        public List<Guid> PaymentMethodIDs { get; set; } = new();
        public List<DateTime> MedicineTimes { get; set; } = new();

        public List<ResidentLookupDTO> GroceryDays { get; set; } = new();
        public List<ResidentLookupDTO> PaymentMethods { get; set; } = new();

        public bool IsActive { get; set; }

        public Guid ResidentStatusID { get; set; }
        public DateTime Date { get; set; }
        public string? RiskLevel { get; set; }
        public string? LatestStatusNote { get; set; }
        public string? GroceryDay { get; set; }
        public string? PaymentMethod { get; set; }
        public List<string> AssignedStaff { get; set; } = new();
        public List<MedicineScheduleItemDto> MedicineSchedule { get; set; } = new();
        public List<PNEntryDto> PNEntries { get; set; } = new();
    }
}
