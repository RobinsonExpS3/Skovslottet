namespace Slottet.Shared
{
    public class ShiftBoardEntryDTO
    {
        public Guid ShiftBoardID { get; set; }
        public string ShiftType { get; set; } = string.Empty;
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
    }
}
