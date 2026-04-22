using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Shared {
    public class StaffDTO {
        public Guid StaffID { get; set; }
        public string StaffName { get; set; }
        public string Initials { get; set; }
        public string Role { get; set; }

        public Guid DepartmentID { get; set; }
    }
}
