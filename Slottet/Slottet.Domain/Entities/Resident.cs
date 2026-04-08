using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Domain.Entities {
    public class Resident {
        public Guid ResidentID { get; set; }
        public string ResidentName { get; set; }
        public string GroceryDay { get; set; }
        public bool IsActive { get; set; }

        public GroceryDay GroceryDayID { get; set; }
    }
}
