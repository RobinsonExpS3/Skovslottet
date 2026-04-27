using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Domain.Entities {
    public class SpecialResponsibilityShiftBoard {
        public Guid ShiftBoardID { get; set; }
        public Guid SpecialResponsibilityID { get; set; }

        public ShiftBoard ShiftBoard { get; set; }
        public SpecialResponsibility SpecialResponsibility { get; set; }
    }
}
