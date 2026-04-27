using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Application.Interfaces {
    public interface IAuditScope {
        public Guid? PerformedByStaffID { get; set; }
        public string? PerformedByStaffName { get; set; }
        public DateTime? PerformedAtTime { get; set; }
    }
}
