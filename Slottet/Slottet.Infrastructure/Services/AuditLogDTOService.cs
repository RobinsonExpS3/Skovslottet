using Microsoft.EntityFrameworkCore;
using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Data;
using Slottet.Shared;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Slottet.Infrastructure.Services {
    public class AuditLogDTOService : IAuditLogDTOService {
        private readonly SlottetDBContext _context;

        public AuditLogDTOService(SlottetDBContext context) {
            _context = context;
        }

        /// <summary>
        /// Sends a query to the database to retrieve all audit log objects and maps them to DTO objects.
        /// </summary>
        /// <returns>Returns a list of AuditLogDTO objects.</returns>
        public async Task<IEnumerable<AuditLogDTO>> GetAllAuditLogsAsync() {
            return await _context.AuditLogs
                    .AsNoTracking()
                    .OrderBy(s => s.AuditLogID)
                    .Select(MapToDtoExpression())
                    .ToListAsync();
        }

        /// <summary>
        /// Sends a query to the database to retrieve audit log objects filtered by date and shift.
        /// </summary>
        /// <param name="date">The date to filter audit logs by, or null to include all dates.</param>
        /// <param name="shift">The shift name to filter audit logs by, or null to include all shifts.</param>
        /// <returns>Returns a filtered list of AuditLogDTO objects.</returns>
        public async Task<IEnumerable<AuditLogDTO>> GetAllAuditLogsAsync(DateOnly? date, string? shift) {
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

        /// <summary>
        /// Creates an expression that maps an AuditLog entity to an AuditLogDTO for use in LINQ queries.
        /// </summary>
        /// <remarks>This expression can be used with LINQ providers such as Entity Framework to perform
        /// efficient server-side projection of AuditLog entities to AuditLogDTO objects.</remarks>
        /// <returns>An expression that projects an AuditLog object into an AuditLogDTO instance.</returns>
        private static Expression<Func<AuditLog, AuditLogDTO>> MapToDtoExpression() {
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
