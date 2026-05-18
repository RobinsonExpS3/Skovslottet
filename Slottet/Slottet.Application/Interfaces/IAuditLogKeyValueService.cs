using Slottet.Shared;

namespace Slottet.Application.Interfaces {
    public interface IAuditLogKeyValueService {
        Task EnrichAuditLogsAsync(IList<AuditLogDto> auditLogs);
    }
}
