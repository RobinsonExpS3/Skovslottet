using Slottet.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Application.Interfaces {
    public interface IAuditLogDTOService {
        Task<IEnumerable<AuditLogDTO>> GetAllAuditLogsAsync();
        Task<IEnumerable<AuditLogDTO>> GetAllAuditLogsAsync(DateOnly? date, string? shift);
    }
}
