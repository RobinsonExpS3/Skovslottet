using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Domain.Entities
{
    public class ResidentMedicine {
        public Medicine MedicineID { get; set; }
        public Resident ResidentID { get; set; }
        public Medicine MedicineTime { get; set; }
        public Medicine MedicinGivenTime { get; set; }
        public Medicine MedicineRegisteredTime { get; set; }

    }
}
