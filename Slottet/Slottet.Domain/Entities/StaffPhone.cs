using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Domain.Entities
{
    public class StaffPhone
    {
        public Guid StaffID { get; set; }
        public Guid PhoneID { get; set; }
        public Guid ShiftBoardID { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.Now;

        public Staff Staff { get; set; }
        public Phone Phone { get; set; }
        public ShiftBoard ShiftBoard { get; set; }
    }
}
