using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Domain.Entities {
    public class ShiftBoard {
        public Guid ShiftBoardID { get; set; }
        public string ShiftType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}