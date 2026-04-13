using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Domain.Entities {
    public class Phone {
        public Guid PhoneID { get; set; }
        public string PhoneNumber { get; set; }

        public Guid DepartmentID { get; set; }
        public Department Department { get; set; }
    }
}
