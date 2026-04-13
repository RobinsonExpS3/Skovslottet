using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Domain.Entities {
    public class SpecialResponsibility {
        public Guid SpecialResponsibilityID { get; set; }
        public string TaskName { get; set; }

        public ShiftBoard ShiftBoardID { get; set; }
    }
}
