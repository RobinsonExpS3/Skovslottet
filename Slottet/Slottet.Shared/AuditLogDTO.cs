using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Shared {
    public class AuditLogDTO {
        public Guid AuditLogID { get; set; }
        public DateTime PerformedAtTime { get; set; }

        public string Action { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public string KeyValues { get; set; } = string.Empty;

        public string? OldValuesJson { get; set; }
        public string? NewValuesJson { get; set; }

        public Guid PerformedByStaffID { get; set; }
        public string PerformedByStaffName { get; set; } = string.Empty;
    }
}
