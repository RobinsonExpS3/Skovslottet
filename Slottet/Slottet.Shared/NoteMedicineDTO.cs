using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Shared
{
    public class NoteMedicineDTO
    {
        public Guid? MedicineID { get; set; }
        public Guid ResidentID { get; set; }
        public DateTime MedicineTime { get; set; }
        public DateTime? MedicineGivenTime { get; set; }
        public DateTime? MedicineRegisteredTime { get; set; }
    }
}
