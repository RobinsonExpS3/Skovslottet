using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Auditing {
    public class AuditInterceptor : SaveChangesInterceptor {
        private readonly IAuditScope _auditScope;

        public AuditInterceptor(IAuditScope auditScope) {
            _auditScope = auditScope;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default) {
            var context = eventData.Context;
            if (context == null) {
                return base.SavingChangesAsync(eventData, result, cancellationToken);
            }

            context.ChangeTracker.DetectChanges();

            var logs = CreateAuditLogs(context);
            if(logs.Count > 0) {
                context.Set<AuditLog>().AddRange(logs);
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);

        }

        private List<AuditLog> CreateAuditLogs(DbContext context) {
            var logs = new List<AuditLog>();
            var Now = DateTime.Now;

            foreach(var entry in context.ChangeTracker.Entries().Where(e => e.Entity is not AuditLog 
                            && e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)) {
                var keyValues = new Dictionary<string, object?>();
                foreach(var property in entry.Properties.Where(p => p.Metadata.IsPrimaryKey())) {
                    keyValues[property.Metadata.Name] = property.CurrentValue;
                }

                var oldValues = new Dictionary<string, object?>();
                var newValues = new Dictionary<string, object?>();

                foreach(var property in entry.Properties) {
                    if(property.Metadata.IsPrimaryKey()) {
                        continue;
                    }

                    switch(entry.State) {
                        case EntityState.Added:
                            newValues[property.Metadata.Name] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            oldValues[property.Metadata.Name] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified) {
                                oldValues[property.Metadata.Name] = property.OriginalValue;
                                newValues[property.Metadata.Name] = property.CurrentValue;
                            }
                            break;
                    }
                }

                logs.Add(new AuditLog {
                    AuditLogID = Guid.NewGuid(),
                    TimeStamp = DateTime.Now,
                    Action = entry.State.ToString(),
                    TableName = entry.Metadata.GetTableName() ?? entry.Entity.GetType().Name,
                    KeyValues = JsonSerializer.Serialize(keyValues),
                    OldValuesJson = oldValues.Count == 0 ? null : JsonSerializer.Serialize(oldValues),
                    NewValuesJson = newValues.Count == 0 ? null : JsonSerializer.Serialize(newValues),
                    PerformedByStaffID = _auditScope.PerformedByStaffID,
                    PerformedByStaffName = _auditScope.PerformedByStaffName,
                    PerformedAtTime = _auditScope.PerformedAtTime,
                });
            }

            return logs;
        }
    }
}
