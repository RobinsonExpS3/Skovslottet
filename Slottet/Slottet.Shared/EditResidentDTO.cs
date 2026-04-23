using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Shared {
    public class EditResidentDTO {
        public Guid ResidentID { get; set; }
        public string ResidentName { get; set; }

        public Guid GroceryDayID { get; set; }

        public List<Guid> PaymentMethodIDs { get; set; } = new();
        public List<DateTime> MedicineTimes { get; set; } = new();
        public List<Guid> PNID { get; set; } = new();

        public List<ResidentLookupDTO> GroceryDays { get; set; } = new();
        public List<ResidentLookupDTO> PaymentMethods { get; set; } = new();

        public bool IsActive { get; set; }
    }
}
