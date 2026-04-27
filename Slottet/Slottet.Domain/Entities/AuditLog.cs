using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Domain.Entities {
    public class AuditLog {
        public Guid AuditLogID { get; set; }
        public DateTime TimeStamp { get; set; }

        public string Action { get; set; }
        public string TableName { get; set; }
        public string KeyValues { get; set; }
        public string? OldValuesJson { get; set; }
        public string? NewValuesJson { get; set; }

        public Guid? PerformedByStaffID { get; set; }
        public string? PerformedByStaffName { get; set; }
        public DateTime? PerformedAtTime { get; set; }
    }
}
