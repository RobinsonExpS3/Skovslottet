namespace Slottet.Shared
{
    public class PhoneDTO
    {
        public Guid PhoneID { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public Guid DepartmentID { get; set; }
    }
}
