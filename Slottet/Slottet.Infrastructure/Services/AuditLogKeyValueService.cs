using Microsoft.EntityFrameworkCore;
using Slottet.Application.Interfaces;
using Slottet.Infrastructure.Data;
using Slottet.Shared;
using System.Text.Json;

namespace Slottet.Infrastructure.Services {
    public class AuditLogKeyValueService : IAuditLogKeyValueService {
        private readonly SlottetDBContext _context;

        public AuditLogKeyValueService(SlottetDBContext context) {
            _context = context;
        }

        // ── Public entry point ────────────────────────────────────────────

        public async Task EnrichAuditLogsAsync(IList<AuditLogDTO> auditLogs) {
            if (auditLogs.Count == 0) return;

            var lookups = await BuildLookupsAsync(auditLogs);

            foreach (var auditLog in auditLogs) {
                var keyValues = ParseAuditValues(auditLog.KeyValues);

                auditLog.Category      = ResolveCategory(auditLog.TableName, auditLog.Action, auditLog.OldValuesJson, auditLog.NewValuesJson);
                ApplyTableSpecificOverrides(auditLog, keyValues, lookups);
                if (string.IsNullOrWhiteSpace(auditLog.Subject))
                    auditLog.Subject   = ResolveSubject(auditLog.TableName, keyValues, lookups);
                auditLog.OldValuesJson = ResolveValuesJson(auditLog.OldValuesJson, lookups);
                auditLog.NewValuesJson = ResolveValuesJson(auditLog.NewValuesJson, lookups);

                if (auditLog.ShiftBoardID.HasValue)
                    auditLog.ShiftBoardLabel = lookups.ShiftBoards.GetValueOrDefault(auditLog.ShiftBoardID.Value, "Ukendt vagt");
            }
        }

        // ── Batch lookup builder ──────────────────────────────────────────

        private async Task<AuditLookups> BuildLookupsAsync(IList<AuditLogDTO> auditLogs) {
            var lookups      = new AuditLookups();
            var guidsByProp  = CollectGuidsByProperty(auditLogs);

            // ShiftBoards — from both scope ID and FK values
            var shiftBoardIds = new HashSet<Guid>(
                auditLogs.Where(l => l.ShiftBoardID.HasValue).Select(l => l.ShiftBoardID!.Value));
            if (guidsByProp.TryGetValue("ShiftBoardID", out var sbPropIds))
                foreach (var id in sbPropIds) shiftBoardIds.Add(id);
            if (shiftBoardIds.Count > 0) {
                var rows = await _context.ShiftBoards.AsNoTracking()
                    .Where(b => shiftBoardIds.Contains(b.ShiftBoardID))
                    .Select(b => new { b.ShiftBoardID, b.ShiftType, b.StartDateTime })
                    .ToListAsync();
                foreach (var b in rows)
                    lookups.ShiftBoards[b.ShiftBoardID] = $"{b.StartDateTime:d/M} {b.ShiftType}";
            }

            // Staffs
            if (guidsByProp.TryGetValue("StaffID", out var staffIds) && staffIds.Count > 0) {
                var rows = await _context.Staffs.AsNoTracking()
                    .Where(s => staffIds.Contains(s.StaffID))
                    .Select(s => new { s.StaffID, s.StaffName })
                    .ToListAsync();
                foreach (var s in rows) lookups.Staff[s.StaffID] = s.StaffName;
            }

            // Residents
            if (guidsByProp.TryGetValue("ResidentID", out var residentIds) && residentIds.Count > 0) {
                var rows = await _context.Residents.AsNoTracking()
                    .Where(r => residentIds.Contains(r.ResidentID))
                    .Select(r => new { r.ResidentID, r.ResidentName })
                    .ToListAsync();
                foreach (var r in rows) lookups.Residents[r.ResidentID] = r.ResidentName;
            }

            // Medicines
            if (guidsByProp.TryGetValue("MedicineID", out var medicineIds) && medicineIds.Count > 0) {
                var rows = await _context.Medicines.AsNoTracking()
                    .Where(m => medicineIds.Contains(m.MedicineID))
                    .Select(m => new { m.MedicineID, ResidentName = m.Resident.ResidentName, m.ScheduledTime })
                    .ToListAsync();
                foreach (var m in rows)
                    lookups.Medicines[m.MedicineID] = $"{m.ResidentName} - Kl. {m.ScheduledTime:HH:mm}";
            }

            // PNs
            if (guidsByProp.TryGetValue("PNID", out var pnIds) && pnIds.Count > 0) {
                var rows = await _context.PNs.AsNoTracking()
                    .Where(p => pnIds.Contains(p.PNID))
                    .Select(p => new { p.PNID, ResidentName = p.Resident.ResidentName, p.PNGivenTime })
                    .ToListAsync();
                foreach (var p in rows)
                    lookups.PNs[p.PNID] = p.ResidentName;
            }

            // ResidentStatuses — two formats: subject (name only) and FK value (full label)
            if (guidsByProp.TryGetValue("ResidentStatusID", out var statusIds) && statusIds.Count > 0) {
                var rows = await _context.ResidentStatuses.AsNoTracking()
                    .Where(s => statusIds.Contains(s.ResidentStatusID))
                    .Select(s => new { s.ResidentStatusID, ResidentName = s.Resident.ResidentName, s.Date })
                    .ToListAsync();
                foreach (var s in rows) {
                    lookups.ResidentStatusSubjects[s.ResidentStatusID] = s.ResidentName;
                    lookups.ResidentStatusValues[s.ResidentStatusID]   = $"{s.ResidentName} - status {s.Date:dd-MM-yyyy HH:mm}";
                }
            }

            // SpecialResponsibilities
            if (guidsByProp.TryGetValue("SpecialResponsibilityID", out var srIds) && srIds.Count > 0) {
                var rows = await _context.SpecialResponsibilities.AsNoTracking()
                    .Where(s => srIds.Contains(s.SpecialResponsibilityID))
                    .Select(s => new { s.SpecialResponsibilityID, s.TaskName })
                    .ToListAsync();
                foreach (var s in rows)
                    lookups.SpecialResponsibilities[s.SpecialResponsibilityID] = s.TaskName;
            }

            // Departments
            if (guidsByProp.TryGetValue("DepartmentID", out var deptIds) && deptIds.Count > 0) {
                var rows = await _context.Departments.AsNoTracking()
                    .Where(d => deptIds.Contains(d.DepartmentID))
                    .Select(d => new { d.DepartmentID, d.DepartmentName })
                    .ToListAsync();
                foreach (var d in rows)
                    lookups.Departments[d.DepartmentID] = d.DepartmentName;
            }

            // DepartmentTasks
            if (guidsByProp.TryGetValue("DepartmentTaskID", out var taskIds) && taskIds.Count > 0) {
                var rows = await _context.DepartmentTasks.AsNoTracking()
                    .Where(t => taskIds.Contains(t.DepartmentTaskID))
                    .Select(t => new { t.DepartmentTaskID, t.DepartmentTaskName })
                    .ToListAsync();
                foreach (var t in rows)
                    lookups.DepartmentTasks[t.DepartmentTaskID] = t.DepartmentTaskName;
            }

            // GroceryDays
            if (guidsByProp.TryGetValue("GroceryDayID", out var groceryIds) && groceryIds.Count > 0) {
                var rows = await _context.GroceryDays.AsNoTracking()
                    .Where(g => groceryIds.Contains(g.GroceryDayID))
                    .Select(g => new { g.GroceryDayID, g.GroceryDayName })
                    .ToListAsync();
                foreach (var g in rows)
                    lookups.GroceryDays[g.GroceryDayID] = g.GroceryDayName;
            }

            // PaymentMethods
            if (guidsByProp.TryGetValue("PaymentMethodID", out var pmIds) && pmIds.Count > 0) {
                var rows = await _context.PaymentMethods.AsNoTracking()
                    .Where(p => pmIds.Contains(p.PaymentMethodID))
                    .Select(p => new { p.PaymentMethodID, p.PaymentMethodName })
                    .ToListAsync();
                foreach (var p in rows)
                    lookups.PaymentMethods[p.PaymentMethodID] = p.PaymentMethodName;
            }

            // Phones
            if (guidsByProp.TryGetValue("PhoneID", out var phoneIds) && phoneIds.Count > 0) {
                var rows = await _context.Phones.AsNoTracking()
                    .Where(p => phoneIds.Contains(p.PhoneID))
                    .Select(p => new { p.PhoneID, p.PhoneNumber })
                    .ToListAsync();
                foreach (var p in rows)
                    lookups.Phones[p.PhoneID] = p.PhoneNumber;
            }

            // RiskLevels
            if (guidsByProp.TryGetValue("RiskLevelID", out var riskIds) && riskIds.Count > 0) {
                var rows = await _context.RiskLevels.AsNoTracking()
                    .Where(r => riskIds.Contains(r.RiskLevelID))
                    .Select(r => new { r.RiskLevelID, r.RiskLevelName })
                    .ToListAsync();
                foreach (var r in rows)
                    lookups.RiskLevels[r.RiskLevelID] = r.RiskLevelName;
            }

            return lookups;
        }

        private static Dictionary<string, HashSet<Guid>> CollectGuidsByProperty(IList<AuditLogDTO> auditLogs) {
            var result = new Dictionary<string, HashSet<Guid>>(StringComparer.OrdinalIgnoreCase);
            foreach (var log in auditLogs) {
                CollectGuidsFromJson(log.KeyValues, result);
                CollectGuidsFromJson(log.OldValuesJson, result);
                CollectGuidsFromJson(log.NewValuesJson, result);
            }
            return result;
        }

        private static void CollectGuidsFromJson(string? json, Dictionary<string, HashSet<Guid>> result) {
            if (string.IsNullOrWhiteSpace(json)) return;
            try {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.ValueKind != JsonValueKind.Object) return;
                foreach (var prop in doc.RootElement.EnumerateObject()) {
                    if (prop.Value.ValueKind == JsonValueKind.String &&
                        Guid.TryParse(prop.Value.GetString(), out var guid)) {
                        if (!result.TryGetValue(prop.Name, out var set)) {
                            set = new HashSet<Guid>();
                            result[prop.Name] = set;
                        }
                        set.Add(guid);
                    }
                }
            } catch (JsonException) { }
        }

        // ── Table-specific overrides (synchronous) ────────────────────────

        private static void ApplyTableSpecificOverrides(AuditLogDTO auditLog, IReadOnlyDictionary<string, string?> keyValues, AuditLookups lookups) {

            // ── MedicineLogs ──────────────────────────────────────────────────
            if (auditLog.TableName == "MedicineLogs") {
                var newVals = ParseAuditValues(auditLog.NewValuesJson);
                var oldVals = ParseAuditValues(auditLog.OldValuesJson);

                // Resolve subject via MedicineID in values
                var medicineIdStr = newVals.GetValueOrDefault("MedicineID") ?? oldVals.GetValueOrDefault("MedicineID");
                if (medicineIdStr != null && Guid.TryParse(medicineIdStr, out var medicineGuid))
                    auditLog.Subject = lookups.Medicines.GetValueOrDefault(medicineGuid, "Ukendt medicin");

                // Determine action from GivenTime presence
                newVals.TryGetValue("GivenTime", out var newGivenTime);
                oldVals.TryGetValue("GivenTime", out var oldGivenTime);
                bool givenNow = !string.IsNullOrWhiteSpace(newGivenTime);
                bool wasGiven = !string.IsNullOrWhiteSpace(oldGivenTime);

                auditLog.DisplayAction = givenNow ? "Gav medicin"
                    : wasGiven             ? "Fjernede medicin"
                    : auditLog.Action == "Added" ? "Gav medicin"
                    : "Fjernede medicin";

                // Show Givet/Ikke givet — field label suppressed via MedicinStatus key
                if (givenNow) {
                    auditLog.NewValuesJson = JsonSerializer.Serialize(new Dictionary<string, string?> { { "MedicinStatus", "Givet" } });
                    auditLog.OldValuesJson = JsonSerializer.Serialize(new Dictionary<string, string?> { { "MedicinStatus", "Ikke givet" } });
                } else if (wasGiven) {
                    auditLog.OldValuesJson = JsonSerializer.Serialize(new Dictionary<string, string?> { { "MedicinStatus", "Givet" } });
                    auditLog.NewValuesJson = JsonSerializer.Serialize(new Dictionary<string, string?> { { "MedicinStatus", "Ikke givet" } });
                } else if (auditLog.Action == "Added") {
                    auditLog.NewValuesJson = JsonSerializer.Serialize(new Dictionary<string, string?> { { "MedicinStatus", "Givet" } });
                    auditLog.OldValuesJson = JsonSerializer.Serialize(new Dictionary<string, string?> { { "MedicinStatus", "Ikke givet" } });
                } else {
                    auditLog.OldValuesJson = JsonSerializer.Serialize(new Dictionary<string, string?> { { "MedicinStatus", "Givet" } });
                    auditLog.NewValuesJson = JsonSerializer.Serialize(new Dictionary<string, string?> { { "MedicinStatus", "Ikke givet" } });
                }
                return;
            }

            // ── Medicines Added/Deleted — collapse to single row ──────────────────
            if (auditLog.TableName == "Medicines") {
                if (auditLog.Action == "Added") {
                    var newVals = ParseAuditValues(auditLog.NewValuesJson);
                    newVals.TryGetValue("MedicationName", out var medName);
                    auditLog.NewValuesJson = !string.IsNullOrWhiteSpace(medName)
                        ? JsonSerializer.Serialize(new Dictionary<string, string?> { { "MedicationName", medName } })
                        : null;
                    auditLog.OldValuesJson = null;
                } else if (auditLog.Action == "Deleted") {
                    var oldVals = ParseAuditValues(auditLog.OldValuesJson);

                    // ResidentID kan være i oldVals eller keyValues
                    string? residentName = null;
                    if (oldVals.TryGetValue("ResidentID", out var resIdStr) && Guid.TryParse(resIdStr, out var resId))
                        residentName = lookups.Residents.GetValueOrDefault(resId);
                    if (residentName == null)
                        residentName = LookupValue(keyValues, "ResidentID", lookups.Residents);

                    string? timeLabel = null;
                    if (oldVals.TryGetValue("ScheduledTime", out var scheduledTimeStr) &&
                        !string.IsNullOrWhiteSpace(scheduledTimeStr) &&
                        TimeOnly.TryParse(scheduledTimeStr, out var t))
                        timeLabel = $"Kl. {t:HH:mm}";

                    auditLog.Subject = (residentName, timeLabel) switch {
                        ({ Length: > 0 }, { Length: > 0 }) => $"{residentName} - {timeLabel}",
                        ({ Length: > 0 }, _)               => residentName!,
                        (_, { Length: > 0 })               => timeLabel!,
                        _                                   => "Arkiveret medicin"
                    };
                    auditLog.OldValuesJson = null;
                    auditLog.NewValuesJson = null;
                }
                return;
            }

            // ── PNs Added — collapse to single row med tidspunkt + medicin + årsag ──
            if (auditLog.TableName == "PNs" && auditLog.Action == "Added") {
                var newVals = ParseAuditValues(auditLog.NewValuesJson);
                newVals.TryGetValue("PNMedication", out var medication);
                newVals.TryGetValue("PNReason", out var reason);
                newVals.TryGetValue("PNGivenTime", out var givenTimeStr);
                string? timeLabel = null;
                if (!string.IsNullOrWhiteSpace(givenTimeStr) && DateTimeOffset.TryParse(givenTimeStr, out var dto))
                    timeLabel = $"Kl. {dto:HH:mm}";
                var combined = string.Join(" · ", new[] { timeLabel, medication, reason }
                    .Where(v => !string.IsNullOrWhiteSpace(v)));
                auditLog.NewValuesJson = !string.IsNullOrWhiteSpace(combined)
                    ? JsonSerializer.Serialize(new Dictionary<string, string?> { { "PNDetaljer", combined } })
                    : null;
                auditLog.OldValuesJson = null;
            }

            // ── StaffPNs ──────────────────────────────────────────────────────
            if (auditLog.TableName == "StaffPNs") {
                auditLog.DisplayAction = auditLog.Action switch {
                    "Added"   => "Registrerede",
                    "Deleted" => "Fjernede",
                    _         => string.Empty
                };
                var staffName = LookupValue(keyValues, "StaffID", lookups.Staff);
                if (auditLog.Action == "Added") {
                    auditLog.NewValuesJson = staffName != null
                        ? JsonSerializer.Serialize(new Dictionary<string, string?> { { "RegistreretAf", staffName } })
                        : null;
                    auditLog.OldValuesJson = null;
                } else {
                    auditLog.OldValuesJson = staffName != null
                        ? JsonSerializer.Serialize(new Dictionary<string, string?> { { "RegistreretAf", staffName } })
                        : null;
                    auditLog.NewValuesJson = null;
                }
                return;
            }

            // ── StaffResidentStatuses (Ansvarligt personale) ──────────────────
            if (auditLog.TableName == "StaffResidentStatuses") {
                auditLog.DisplayAction = auditLog.Action switch {
                    "Added"   => "Tildelte",
                    "Deleted" => "Fjernede",
                    _         => string.Empty
                };
                // Subject = resident name via ResidentStatusID
                if (keyValues.TryGetValue("ResidentStatusID", out var rsIdStr) &&
                    Guid.TryParse(rsIdStr, out var rsGuid))
                    auditLog.Subject = lookups.ResidentStatusSubjects.GetValueOrDefault(rsGuid, string.Empty);

                var staffName = LookupValue(keyValues, "StaffID", lookups.Staff);
                if (auditLog.Action == "Added") {
                    auditLog.NewValuesJson = staffName != null
                        ? JsonSerializer.Serialize(new Dictionary<string, string?> { { "AnsvarligtPersonale", staffName } })
                        : null;
                    auditLog.OldValuesJson = null;
                } else {
                    auditLog.OldValuesJson = staffName != null
                        ? JsonSerializer.Serialize(new Dictionary<string, string?> { { "AnsvarligtPersonale", staffName } })
                        : null;
                    auditLog.NewValuesJson = null;
                }
                return;
            }

            // ── SpecialResponsibilities (entiteten) ───────────────────────────────────
            if (auditLog.TableName == "SpecialResponsibilities") {
                // Subject — altid "Særligt Ansvar" så entitets-ændringer samles med assignments
                auditLog.Subject = "Særligt Ansvar";
                // Kollaps værdier til kun TaskName
                if (auditLog.Action == "Added") {
                    var newVals = ParseAuditValues(auditLog.NewValuesJson);
                    newVals.TryGetValue("TaskName", out var name);
                    auditLog.NewValuesJson = !string.IsNullOrWhiteSpace(name)
                        ? JsonSerializer.Serialize(new Dictionary<string, string?> { { "TaskName", name } })
                        : null;
                    auditLog.OldValuesJson = null;
                } else if (auditLog.Action == "Deleted") {
                    var oldVals = ParseAuditValues(auditLog.OldValuesJson);
                    oldVals.TryGetValue("TaskName", out var name);
                    auditLog.OldValuesJson = !string.IsNullOrWhiteSpace(name)
                        ? JsonSerializer.Serialize(new Dictionary<string, string?> { { "TaskName", name } })
                        : null;
                    auditLog.NewValuesJson = null;
                }
                // Modified: behold ændrede felter som de er
            }

            // ── Phones (entiteten) — kollaps til kun Telefonnummer ────────────────
            if (auditLog.TableName == "Phones") {
                auditLog.Subject = "Telefoner";
                if (auditLog.Action == "Added") {
                    var newVals = ParseAuditValues(auditLog.NewValuesJson);
                    newVals.TryGetValue("PhoneNumber", out var num);
                    auditLog.NewValuesJson = !string.IsNullOrWhiteSpace(num)
                        ? JsonSerializer.Serialize(new Dictionary<string, string?> { { "PhoneNumber", num } })
                        : null;
                    auditLog.OldValuesJson = null;
                } else if (auditLog.Action == "Deleted") {
                    var oldVals = ParseAuditValues(auditLog.OldValuesJson);
                    oldVals.TryGetValue("PhoneNumber", out var num);
                    auditLog.OldValuesJson = !string.IsNullOrWhiteSpace(num)
                        ? JsonSerializer.Serialize(new Dictionary<string, string?> { { "PhoneNumber", num } })
                        : null;
                    auditLog.NewValuesJson = null;
                }
                // Modified: behold ændrede felter (viser Telefonnummer fra→til)
            }

            // ── Faste gruppe-navne ────────────────────────────────────────────
            if (auditLog.TableName == "GroceryDays")      auditLog.Subject = "Handledage";
            if (auditLog.TableName == "GroceryDays")      auditLog.Subject = "Handledage";
            if (auditLog.TableName == "Departments")      auditLog.Subject = "Afdelinger";
            if (auditLog.TableName == "DepartmentTasks")  auditLog.Subject = "Afdelingsopgaver";
            if (auditLog.TableName == "PaymentMethods")   auditLog.Subject = "Betalingsmetoder";

            if (auditLog.TableName == "ResidentStatuses" && auditLog.Action == "Added") {
                var values   = ParseAuditValues(auditLog.NewValuesJson);
                var filtered = new Dictionary<string, string?>();
                if (values.TryGetValue("Status", out var statusVal) && !string.IsNullOrWhiteSpace(statusVal))
                    filtered["Status"] = statusVal;
                if (values.TryGetValue("RiskLevelID", out var riskVal))
                    filtered["RiskLevelID"] = riskVal;
                auditLog.NewValuesJson = filtered.Count > 0 ? JsonSerializer.Serialize(filtered) : null;
                auditLog.OldValuesJson = null;
            }

            if (auditLog.TableName == "StaffPhones") {
                auditLog.Subject = "Telefoner";
                auditLog.DisplayAction = auditLog.Action switch {
                    "Added"   => "Tildelte",
                    "Deleted" => "Fjernede",
                    _         => string.Empty
                };
                var staffName = LookupValue(keyValues, "StaffID", lookups.Staff);
                if (keyValues.TryGetValue("PhoneID", out var phoneId) &&
                    Guid.TryParse(phoneId, out var phoneGuid)) {
                    var phoneNumber = lookups.Phones.GetValueOrDefault(phoneGuid);
                    var fieldKey   = phoneNumber ?? "Telefon";
                    if (auditLog.Action == "Added") {
                        auditLog.NewValuesJson = JsonSerializer.Serialize(new Dictionary<string, string?> { { fieldKey, staffName } });
                        auditLog.OldValuesJson = null;
                    } else if (auditLog.Action == "Deleted") {
                        auditLog.OldValuesJson = JsonSerializer.Serialize(new Dictionary<string, string?> { { fieldKey, staffName } });
                        auditLog.NewValuesJson = null;
                    }
                }
            }

            if (auditLog.TableName == "SpecialResponsibilityStaffs") {
                auditLog.Subject = "Særligt Ansvar";
                auditLog.DisplayAction = auditLog.Action switch {
                    "Added"   => "Tildelte",
                    "Deleted" => "Fjernede",
                    _         => string.Empty
                };
                var respName  = LookupValue(keyValues, "SpecialResponsibilityID", lookups.SpecialResponsibilities);
                var staffName = LookupValue(keyValues, "StaffID", lookups.Staff);
                var fieldKey  = !string.IsNullOrWhiteSpace(respName) ? respName : "SærligtAnsvar";
                if (auditLog.Action == "Added") {
                    auditLog.NewValuesJson = JsonSerializer.Serialize(new Dictionary<string, string?> { { fieldKey, staffName } });
                    auditLog.OldValuesJson = null;
                } else {
                    auditLog.OldValuesJson = JsonSerializer.Serialize(new Dictionary<string, string?> { { fieldKey, staffName } });
                    auditLog.NewValuesJson = null;
                }
            }
        }

        // ── Subject resolution (synchronous) ─────────────────────────────

        private static string ResolveSubject(string tableName, IReadOnlyDictionary<string, string?> keyValues, AuditLookups lookups) {
            if (keyValues.Count == 0) return "Ikke givet";

            return tableName switch {
                "ShiftBoards"                 => LookupSubject(keyValues, "ShiftBoardID",            lookups.ShiftBoards,            lookups),
                "Staffs"                      => LookupSubject(keyValues, "StaffID",                 lookups.Staff,                  lookups),
                "Residents"                   => LookupSubject(keyValues, "ResidentID",              lookups.Residents,              lookups),
                "Medicines"                   => LookupSubject(keyValues, "MedicineID",              lookups.Medicines,              lookups),
                "PNs"                         => LookupSubject(keyValues, "PNID",                    lookups.PNs,                    lookups),
                "ResidentStatuses"            => LookupSubject(keyValues, "ResidentStatusID",        lookups.ResidentStatusSubjects, lookups),
                "SpecialResponsibilities"     => LookupSubject(keyValues, "SpecialResponsibilityID", lookups.SpecialResponsibilities, lookups),
                "SpecialResponsibilityStaffs" => LookupSubject(keyValues, "StaffID",                lookups.Staff,                  lookups),
                "StaffPhones"                 => LookupSubject(keyValues, "StaffID",                lookups.Staff,                  lookups),
                "Departments"                 => LookupSubject(keyValues, "DepartmentID",            lookups.Departments,            lookups),
                "DepartmentTasks"             => LookupSubject(keyValues, "DepartmentTaskID",        lookups.DepartmentTasks,        lookups),
                "GroceryDays"                 => LookupSubject(keyValues, "GroceryDayID",            lookups.GroceryDays,            lookups),
                "PaymentMethods"              => LookupSubject(keyValues, "PaymentMethodID",         lookups.PaymentMethods,         lookups),
                "Phones"                      => LookupSubject(keyValues, "PhoneID",                 lookups.Phones,                 lookups),
                "RiskLevels"                  => LookupSubject(keyValues, "RiskLevelID",             lookups.RiskLevels,             lookups),
                "StaffPNs"                    => LookupSubject(keyValues, "PNID",                    lookups.PNs,                    lookups),
                "StaffResidentStatuses"       => LookupSubject(keyValues, "ResidentStatusID",        lookups.ResidentStatusSubjects, lookups),
                "ResidentPaymentMethods"      => string.Join(" - ", new[] {
                                                    LookupValue(keyValues, "ResidentID",      lookups.Residents),
                                                    LookupValue(keyValues, "PaymentMethodID", lookups.PaymentMethods)
                                                }.Where(v => !string.IsNullOrWhiteSpace(v))),
                _                             => ResolveFallbackSubject(keyValues, lookups)
            };
        }

        private static string LookupSubject(IReadOnlyDictionary<string, string?> keyValues, string key, Dictionary<Guid, string> lookup, AuditLookups lookups) {
            var value = LookupValue(keyValues, key, lookup);
            return string.IsNullOrWhiteSpace(value) ? ResolveFallbackSubject(keyValues, lookups) : value;
        }

        private static string? LookupValue(IReadOnlyDictionary<string, string?> keyValues, string key, Dictionary<Guid, string> lookup) {
            return keyValues.TryGetValue(key, out var raw) && Guid.TryParse(raw, out var guid)
                ? lookup.GetValueOrDefault(guid)
                : null;
        }

        private static string ResolveFallbackSubject(IReadOnlyDictionary<string, string?> keyValues, AuditLookups lookups) {
            var parts = keyValues
                .Select(kv => Guid.TryParse(kv.Value, out var guid)
                    ? lookups.ResolveAnyGuid(kv.Key, guid)   // null → filtreres fra (ingen rå GUIDs)
                    : kv.Value)
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .ToArray();
            return JoinSubjectParts(parts);
        }

        // ── Value JSON resolution (synchronous) ───────────────────────────

        private static string? ResolveValuesJson(string? json, AuditLookups lookups) {
            var values = ParseAuditValues(json);
            if (values.Count == 0) return json;

            var displayValues = new Dictionary<string, string?>();
            foreach (var kv in values)
                displayValues[kv.Key] = ResolveValue(kv.Key, kv.Value, lookups);

            return JsonSerializer.Serialize(displayValues);
        }

        private static string? ResolveValue(string propertyName, string? value, AuditLookups lookups) {
            if (string.IsNullOrWhiteSpace(value) || !Guid.TryParse(value, out var id))
                return value;

            var resolved = propertyName switch {
                "ShiftBoardID"            => lookups.ShiftBoards.GetValueOrDefault(id),
                "StaffID"                 => lookups.Staff.GetValueOrDefault(id),
                "ResidentID"              => lookups.Residents.GetValueOrDefault(id),
                "MedicineID"              => lookups.Medicines.GetValueOrDefault(id),
                "PNID"                    => lookups.PNs.GetValueOrDefault(id),
                "ResidentStatusID"        => lookups.ResidentStatusValues.GetValueOrDefault(id),
                "SpecialResponsibilityID" => lookups.SpecialResponsibilities.GetValueOrDefault(id),
                "DepartmentID"            => lookups.Departments.GetValueOrDefault(id),
                "DepartmentTaskID"        => lookups.DepartmentTasks.GetValueOrDefault(id),
                "GroceryDayID"            => lookups.GroceryDays.GetValueOrDefault(id),
                "PaymentMethodID"         => lookups.PaymentMethods.GetValueOrDefault(id),
                "PhoneID"                 => lookups.Phones.GetValueOrDefault(id),
                "RiskLevelID"             => lookups.RiskLevels.GetValueOrDefault(id),
                _                         => null
            };

            return resolved ?? GetUnknownReferenceText(propertyName);
        }

        // ── Category resolution ───────────────────────────────────────────

        private static string ResolveCategory(string tableName, string action, string? oldValuesJson, string? newValuesJson) {
            return tableName switch {
                // Medicin
                "Medicines"                   => "Medicin",
                "MedicineLogs"                => "Medicin",
                // PN
                "PNs"                         => "PN",
                "StaffPNs"                    => "PN",
                // Særligt Ansvar
                "SpecialResponsibilities"     => "Særligt Ansvar",
                "SpecialResponsibilityStaffs" => "Særligt Ansvar",
                // Beboer
                "ResidentStatuses"            => "Status",
                "StaffResidentStatuses"       => "Borger",
                // Betaling
                "ResidentPaymentMethods"      => "Betalingsmuligheder",
                // Vagttavle
                "ShiftBoards"                 => "Vagttavle",
                // Telefon
                "Phones"                      => "Telefon",
                "StaffPhones"                 => "Telefon",
                // Risiko
                "RiskLevels"                  => "Risikoniveau",
                // Admin — kun for administratorer
                "Staffs"                      => "Admin",
                "Residents"                   => "Admin",
                "Departments"                 => "Admin",
                "DepartmentTasks"             => "Admin",
                "GroceryDays"                 => "Admin",
                "PaymentMethods"              => "Admin",
                _                             => tableName
            };
        }

        // ── Helpers ───────────────────────────────────────────────────────

        private static Dictionary<string, string?> ParseAuditValues(string? json) {
            var values = new Dictionary<string, string?>();
            if (string.IsNullOrWhiteSpace(json)) return values;
            try {
                using var document = JsonDocument.Parse(json);
                if (document.RootElement.ValueKind != JsonValueKind.Object) return values;
                foreach (var property in document.RootElement.EnumerateObject())
                    values[property.Name] = FormatJsonValue(property.Value);
            } catch (JsonException) { }
            return values;
        }

        private static string? FormatJsonValue(JsonElement value) =>
            value.ValueKind switch {
                JsonValueKind.Null   => null,
                JsonValueKind.String => value.GetString(),
                JsonValueKind.True   => "Ja",
                JsonValueKind.False  => "Nej",
                JsonValueKind.Number => value.GetRawText(),
                _                    => value.GetRawText()
            };

        private static string JoinSubjectParts(params string?[] parts) {
            var subject = string.Join(", ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
            return string.IsNullOrWhiteSpace(subject) ? "-" : subject;
        }

        private static string GetUnknownReferenceText(string propertyName) {
            var displayName = propertyName.EndsWith("ID", StringComparison.Ordinal)
                ? propertyName[..^2]
                : propertyName;
            return $"Ukendt {SplitPascalCase(displayName).ToLowerInvariant()}";
        }

        private static string SplitPascalCase(string value) {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            return string.Concat(value.Select((c, i) =>
                i > 0 && char.IsUpper(c) && !char.IsUpper(value[i - 1]) ? $" {c}" : c.ToString()));
        }

        private static bool ContainsKey(string? json, string key) {
            if (string.IsNullOrWhiteSpace(json)) return false;
            try {
                using var doc = JsonDocument.Parse(json);
                return doc.RootElement.ValueKind == JsonValueKind.Object &&
                       doc.RootElement.TryGetProperty(key, out _);
            } catch (JsonException) { return false; }
        }

        // ── Lookup bag ────────────────────────────────────────────────────

        private sealed class AuditLookups {
            public Dictionary<Guid, string> ShiftBoards              { get; } = new();
            public Dictionary<Guid, string> Staff                    { get; } = new();
            public Dictionary<Guid, string> Residents                { get; } = new();
            public Dictionary<Guid, string> Medicines                { get; } = new();
            public Dictionary<Guid, string> PNs                      { get; } = new();
            public Dictionary<Guid, string> ResidentStatusSubjects   { get; } = new();
            public Dictionary<Guid, string> ResidentStatusValues     { get; } = new();
            public Dictionary<Guid, string> SpecialResponsibilities  { get; } = new();
            public Dictionary<Guid, string> Departments              { get; } = new();
            public Dictionary<Guid, string> DepartmentTasks          { get; } = new();
            public Dictionary<Guid, string> GroceryDays              { get; } = new();
            public Dictionary<Guid, string> PaymentMethods           { get; } = new();
            public Dictionary<Guid, string> Phones                   { get; } = new();
            public Dictionary<Guid, string> RiskLevels               { get; } = new();

            public string? ResolveAnyGuid(string propertyName, Guid id) => propertyName switch {
                "ShiftBoardID"            => ShiftBoards.GetValueOrDefault(id),
                "StaffID"                 => Staff.GetValueOrDefault(id),
                "ResidentID"              => Residents.GetValueOrDefault(id),
                "MedicineID"              => Medicines.GetValueOrDefault(id),
                "PNID"                    => PNs.GetValueOrDefault(id),
                "ResidentStatusID"        => ResidentStatusValues.GetValueOrDefault(id),
                "SpecialResponsibilityID" => SpecialResponsibilities.GetValueOrDefault(id),
                "DepartmentID"            => Departments.GetValueOrDefault(id),
                "DepartmentTaskID"        => DepartmentTasks.GetValueOrDefault(id),
                "GroceryDayID"            => GroceryDays.GetValueOrDefault(id),
                "PaymentMethodID"         => PaymentMethods.GetValueOrDefault(id),
                "PhoneID"                 => Phones.GetValueOrDefault(id),
                "RiskLevelID"             => RiskLevels.GetValueOrDefault(id),
                _                         => null
            };
        }
    }
}
