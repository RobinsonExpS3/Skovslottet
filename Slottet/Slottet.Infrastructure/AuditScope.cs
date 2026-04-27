using Slottet.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Infrastructure {
    public class AuditScope : IAuditScope {
        public Guid? PerformedByStaffID { get; set; }
        public string? PerformedByStaffName { get; set; }
        public DateTime? PerformedAtTime { get; set; }
    }
}
