using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Domain.Entities {
    public class StaffShift {
        public ShiftBoard ShiftBoardID { get; set; }
        public Staff StaffID { get; set; }
    }
}
