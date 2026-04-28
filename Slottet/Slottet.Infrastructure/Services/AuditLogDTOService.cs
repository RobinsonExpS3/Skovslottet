using Microsoft.EntityFrameworkCore;
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

        public AuditLogDTOService(SlottetDBContext context) {
            _context = context;
        }

        public async Task<IEnumerable<AuditLogDTO>> GetAllAsync() {
            return await _context.AuditLogs
                    .AsNoTracking()
                    .OrderBy(s => s.AuditLogID)
                    .Select(MapToDtoExpression())
                    .ToListAsync();
        }

        public async Task<IEnumerable<AuditLogDTO>> GetAllAsync(DateOnly? date, string? shift) {
            IQueryable<AuditLog> query = _context.AuditLogs.AsNoTracking();

            if (date.HasValue) {
                var from = date.Value.ToDateTime(TimeOnly.MinValue);
                var to = from.AddDays(1);

                query = query.Where(log =>
                    (log.PerformedAtTime ?? log.TimeStamp) >= from &&
                    (log.PerformedAtTime ?? log.TimeStamp) < to);
            }

            if (!string.IsNullOrWhiteSpace(shift)) {
                var normalizedShift = shift.Trim().ToLowerInvariant();

                query = normalizedShift switch {
                    "day" => query.Where(log =>
                        (log.PerformedAtTime ?? log.TimeStamp).Hour >= 7 &&
                        (log.PerformedAtTime ?? log.TimeStamp).Hour < 15),
                    "evening" => query.Where(log =>
                        (log.PerformedAtTime ?? log.TimeStamp).Hour >= 15 &&
                        (log.PerformedAtTime ?? log.TimeStamp).Hour < 23),
                    "night" => query.Where(log =>
                        (log.PerformedAtTime ?? log.TimeStamp).Hour >= 23 ||
                        (log.PerformedAtTime ?? log.TimeStamp).Hour < 7),
                    _ => query
                };
            }

            return await query
                .OrderBy(log => log.AuditLogID)
                .Select(MapToDtoExpression())
                .ToListAsync();
        }

        private static System.Linq.Expressions.Expression<Func<AuditLog, AuditLogDTO>> MapToDtoExpression() {
            return auditLog => new AuditLogDTO {
                AuditLogID = auditLog.AuditLogID,
                PerformedAtTime = auditLog.PerformedAtTime ?? auditLog.TimeStamp,
                Action = auditLog.Action,
                TableName = auditLog.TableName,
                KeyValues = auditLog.KeyValues,
                OldValuesJson = auditLog.OldValuesJson,
                NewValuesJson = auditLog.NewValuesJson,
                PerformedByStaffID = auditLog.PerformedByStaffID ?? Guid.Empty,
                PerformedByStaffName = auditLog.PerformedByStaffName ?? "Ukendt"
            };
        }
    }
}
