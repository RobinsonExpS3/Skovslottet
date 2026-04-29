using Slottet.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Application.Interfaces {
    public interface IAuditLogDTOService {
        Task<IEnumerable<AuditLogDTO>> GetAllAsync();
        Task<IEnumerable<AuditLogDTO>> GetAllAsync(DateOnly? date, string? shift);
    }
}
