using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Domain.Entities
{
    public class StaffPhone
    {
        public Guid StaffID { get; set; }
        public Staff Staff { get; set; }

        public Guid PhoneID { get; set; }
        public Phone Phone { get; set; }

        public DateTime AssignedAt { get; set; }
    }
}
