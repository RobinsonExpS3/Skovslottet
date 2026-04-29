using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Domain.Entities {
    public class SpecialResponsibilityStaff {
        public Guid SpecialResponsibilityID { get; set; }
        public Guid StaffID { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.Now;
        public Guid DepartmentID { get; set; }

        public Staff Staff { get; set; }
        public SpecialResponsibility SpecialResponsibility { get; set; }
        public Department Department { get; set; }
    }
}
