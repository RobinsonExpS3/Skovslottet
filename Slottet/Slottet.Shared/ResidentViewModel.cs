using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Shared {
    public class ResidentViewModel {
        public Guid ResidentID { get; set; }
        public string ResidentName { get; set; } = string.Empty;
        public Guid GroceryDayID { get; set; }
        public string GroceryDayName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
