using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Data;
using Slottet.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Infrastructure.Services {
    public class AuditLogDTOService : IAuditLogDTOService {
        private readonly SlottetDBContext _context;

        public StaffDTOService(SlottetDBContext context) {
            _context = context;
        }

        public async Task<IEnumerable<AuditLogDTO>> GetAllAsync() {
            return await _context.AuditLogs
                    .AsNoTracking()
                    .OrderBy(s => s.AuditLogID)
                    .Select(MapToDtoExpression())
                    .ToListAsync();
        }

        private static System.Linq.Expressions.Expression<Func<AuditLog, AuditLogDTO>> MapToDtoExpression() {
            return auditLog => new AuditLogDTO {
                AuditLogID = auditLog.AuditLogID,
                PerformedAtTime = auditLog.PerformedAtTime,
                Action = auditLog.Action,
                TableName = auditLog.TableName,
                KeyValues = auditLog.KeyValues,
                OldValuesJson = auditLog.OldValuesJson,
                NewValuesJson = auditLog.NewValuesJson,
                PerformedByStaffID = auditLog?.PerformedByStaffID,
                PerformedByStaffName = auditLog?.PerformedByStaffName
            };
        }
    }
}