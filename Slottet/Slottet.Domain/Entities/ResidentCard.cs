using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Domain.Entities {
    public class ResidentCard {
        public Guid ResidentCardID { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public Staff StaffID { get; set; }
        public Resident ResidentID { get; set; }
        public RiskLevel RiskLevelID { get; set; }
        public PN PNID { get; set; }
    }
}
