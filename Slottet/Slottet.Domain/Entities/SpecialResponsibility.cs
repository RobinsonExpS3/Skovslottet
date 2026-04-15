using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Domain.Entities {
    public class SpecialResponsibility
    {
        public Guid SpecialResponsibilityID { get; set; }
        public string TaskName { get; set; }

        public Guid ShiftBoardID { get; set; }
        public ShiftBoard ShiftBoard { get; set; }
    }
}
