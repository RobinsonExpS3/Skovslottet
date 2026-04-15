using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Domain.Entities {
    public class ResidentStatus
    {
        public Guid ResidentStatusID { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }

        public Guid ResidentID { get; set; }
        public Resident Resident { get; set; }
        public Guid RiskLevelID { get; set; }
        public RiskLevel RiskLevel { get; set; }
       

        public ICollection<StaffResidentStatus> StaffResidentStatuses { get; set; }
        public ICollection<PN> PNs { get; set; }
    }

}
