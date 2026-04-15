using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Domain.Entities
{
    public class DepartmentTask 
    {
        public Guid DepartmentTaskID { get; set; }
        public string DepartmentTaskName { get; set; }

        public Guid DepartmentID { get; set; }
        public Department Department { get; set; }
    }
}