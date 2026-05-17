namespace Slottet.Shared
{
    public sealed class MedicineScheduleItemDto
    {
        public Guid   MedicineID { get; set; }
        public string Label      { get; set; } = string.Empty;
        public TimeOnly Time     { get; set; }
        public bool IsGiven      { get; set; }
    }
}
