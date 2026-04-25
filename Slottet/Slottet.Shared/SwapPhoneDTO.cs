using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Shared
{
    public class SwapPhoneDTO
    {
        public Guid PhoneID { get; set; }
        public string? PhoneNumber { get; set; }
        public Guid StaffID { get; set; }
        public string? StaffName { get; set; }
        public DateTime? AssignedAt { get; set; }
    }
}
