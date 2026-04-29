using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Domain.Entities {
    public class Department {
        public Guid DepartmentID { get; set; }
        public string DepartmentName { get; set; }

        public ICollection<DepartmentTask> DepartmentTasks { get; set; }
        public ICollection<Phone> Phones { get; set; }
        public ICollection<Staff> Staffs { get; set; }
    }
}