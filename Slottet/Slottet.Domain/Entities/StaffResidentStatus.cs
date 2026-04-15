using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Domain.Entities
{
    public class StaffResidentStatus
    {
        public Guid StaffID { get; set; }
        public Guid ResidentStatusID { get; set; }

        public Staff Staff { get; set; }
        public ResidentStatus ResidentStatus { get; set; }
    }
}
