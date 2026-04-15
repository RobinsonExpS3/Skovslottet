using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Domain.Entities
{
    public class StaffShift
    {
        public Guid ShiftBoardID { get; set; }
        public ShiftBoard ShiftBoard { get; set; }

        public Guid StaffID { get; set; }
        public Staff Staff { get; set; }
    }
}
