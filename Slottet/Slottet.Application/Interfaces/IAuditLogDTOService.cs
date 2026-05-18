using Slottet.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Application.Interfaces {
    public interface IAuditLogDtoService {
        Task<IEnumerable<AuditLogDto>> GetAllAuditLogsAsync();
        Task<IEnumerable<AuditLogDto>> GetAllAuditLogsAsync(DateOnly? date, string? shift);
    }
}
