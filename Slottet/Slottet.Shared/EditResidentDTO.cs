using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Shared {
    public class EditResidentDto {
        public Guid ResidentID { get; set; }
        public string ResidentName { get; set; }

        public Guid GroceryDayID { get; set; }

        public List<Guid> PaymentMethodIDs { get; set; } = new();
        public List<TimeOnly> MedicineTimes { get; set; } = new();

        public List<ResidentLookupDto> GroceryDays { get; set; } = new();
        public List<ResidentLookupDto> PaymentMethods { get; set; } = new();

        public bool IsActive { get; set; }
    }
}
