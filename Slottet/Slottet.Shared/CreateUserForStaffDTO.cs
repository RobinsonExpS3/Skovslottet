namespace Slottet.Shared
{
    public class CreateUserForStaffDTO
    {
        public Guid StaffID { get; set; }
        public string? UserName { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public string? AuthRole { get; set; } = string.Empty;
    }
}
