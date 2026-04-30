namespace Slottet.Shared
{
    public record SwapPhoneRecDTO
    {
        public Guid PhoneID { get; init; }
        public string? PhoneNumber { get; init; }
        public Guid StaffID { get; init; }
        public string? StaffName { get; init; }
        public DateTime? AssignedAt { get; init; }
    }
}
