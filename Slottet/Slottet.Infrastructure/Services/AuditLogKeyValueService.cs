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

        public async Task EnrichAuditLogsAsync(IList<AuditLogDTO> auditLogs) {
            foreach (var auditLog in auditLogs) {
                var keyValues = ParseAuditValues(auditLog.KeyValues);

                auditLog.Subject = await ResolveSubjectAsync(auditLog.TableName, keyValues);
                auditLog.OldValuesJson = await ResolveAuditValuesJsonAsync(auditLog.OldValuesJson);
                auditLog.NewValuesJson = await ResolveAuditValuesJsonAsync(auditLog.NewValuesJson);
            }
        }

        private async Task<string?> ResolveAuditValuesJsonAsync(string? json) {
            var values = ParseAuditValues(json);
            if (values.Count == 0) {
                return json;
            }

            var displayValues = new Dictionary<string, string?>();
            foreach (var value in values) {
                displayValues[value.Key] = await ResolveValueAsync(value.Key, value.Value);
            }

            return JsonSerializer.Serialize(displayValues);
        }

        /// <summary>
        /// Resolves a human-readable subject for an audit log entry.
        /// </summary>
        /// <param name="tableName">
        /// The name of the table recorded on the audit log (for example "Staffs", "Residents",
        /// "ShiftBoards" etc.). This value is used to select a specialized resolver for common tables.
        /// </param>
        /// <param name="keyValues">
        /// Parsed key/value pairs from the audit log's KeyValues JSON. Keys are property names
        /// (e.g. "StaffID", "ResidentID") and values are string representations (often GUIDs).
        /// </param>
        /// <returns>
        /// A task that resolves to a subject string suitable for display. If no key values are provided
        /// the method returns the Danish text "Ikke givet". If a known table is provided a specialized
        /// resolver is used; otherwise <see cref="ResolveFallbackSubjectAsync(IReadOnlyDictionary{string, string?})"/>
        /// is invoked which attempts to resolve each key individually.
        /// </returns>
        /// <remarks>
        /// Supported table names (mapped to dedicated resolvers):
        /// ShiftBoards, Staffs, Residents, Medicines, PNs, ResidentStatuses, StaffShifts,
        /// StaffResidentStatuses, ResidentPaymentMethods, SpecialResponsibilities,
        /// SpecialResponsibilityStaff, Departments, DepartmentTasks, GroceryDays,
        /// PaymentMethods, Phones, RiskLevels.
        ///
        /// The resolution process may perform asynchronous database lookups via the injected
        /// <see cref="SlottetDBContext"/>. Unknown references are converted to a readable
        /// fallback using <see cref="GetUnknownReferenceText(string)"/>.
        /// </remarks>
        private async Task<string> ResolveSubjectAsync(string tableName, IReadOnlyDictionary<string, string?> keyValues) {
            if (keyValues.Count == 0) {
                return "Ikke givet";
            }

            return tableName switch {
                "ShiftBoards" => await ResolveShiftBoardSubjectAsync(keyValues),
                "Staffs" => await ResolveStaffSubjectAsync(keyValues),
                "Residents" => await ResolveResidentSubjectAsync(keyValues),
                "Medicines" => await ResolveMedicineSubjectAsync(keyValues),
                "PNs" => await ResolvePnSubjectAsync(keyValues),
                "ResidentStatuses" => await ResolveResidentStatusSubjectAsync(keyValues),
                "StaffResidentStatuses" => await ResolveStaffResidentStatusSubjectAsync(keyValues),
                "ResidentPaymentMethods" => await ResolveResidentPaymentMethodSubjectAsync(keyValues),
                "SpecialResponsibilities" => await ResolveSpecialResponsibilitySubjectAsync(keyValues),
                "SpecialResponsibilityStaff" => await ResolveSpecialResponsibilityStaffSubjectAsync(keyValues),
                "Departments" => await ResolveDepartmentSubjectAsync(keyValues),
                "DepartmentTasks" => await ResolveDepartmentTaskSubjectAsync(keyValues),
                "GroceryDays" => await ResolveGroceryDaySubjectAsync(keyValues),
                "PaymentMethods" => await ResolvePaymentMethodSubjectAsync(keyValues),
                "Phones" => await ResolvePhoneSubjectAsync(keyValues),
                "RiskLevels" => await ResolveRiskLevelSubjectAsync(keyValues),
                _ => await ResolveFallbackSubjectAsync(keyValues)
            };
        }

        private async Task<string?> ResolveValueAsync(string propertyName, string? value) {
            if (string.IsNullOrWhiteSpace(value) || !Guid.TryParse(value, out var id)) {
                return value;
            }

            var resolvedValue = propertyName switch {
                "ShiftBoardID" => await ResolveShiftBoardNameAsync(id),
                "StaffID" => await ResolveStaffNameAsync(id),
                "ResidentID" => await ResolveResidentNameAsync(id),
                "MedicineID" => await ResolveMedicineNameAsync(id),
                "PNID" => await ResolvePnNameAsync(id),
                "ResidentStatusID" => await ResolveResidentStatusNameAsync(id),
                "SpecialResponsibilityID" => await ResolveSpecialResponsibilityNameAsync(id),
                "DepartmentID" => await ResolveDepartmentNameAsync(id),
                "DepartmentTaskID" => await ResolveDepartmentTaskNameAsync(id),
                "GroceryDayID" => await ResolveGroceryDayNameAsync(id),
                "PaymentMethodID" => await ResolvePaymentMethodNameAsync(id),
                "PhoneID" => await ResolvePhoneNameAsync(id),
                "RiskLevelID" => await ResolveRiskLevelNameAsync(id),
                _ => null
            };

            return resolvedValue ?? GetUnknownReferenceText(propertyName);
        }

        private async Task<string> ResolveShiftBoardSubjectAsync(IReadOnlyDictionary<string, string?> keyValues) =>
            await ResolveSingleKeySubjectAsync(keyValues, "ShiftBoardID", ResolveShiftBoardNameAsync);

        private async Task<string> ResolveStaffSubjectAsync(IReadOnlyDictionary<string, string?> keyValues) =>
            await ResolveSingleKeySubjectAsync(keyValues, "StaffID", ResolveStaffNameAsync);

        private async Task<string> ResolveResidentSubjectAsync(IReadOnlyDictionary<string, string?> keyValues) =>
            await ResolveSingleKeySubjectAsync(keyValues, "ResidentID", ResolveResidentNameAsync);

        private async Task<string> ResolveMedicineSubjectAsync(IReadOnlyDictionary<string, string?> keyValues) =>
            await ResolveSingleKeySubjectAsync(keyValues, "MedicineID", ResolveMedicineNameAsync);

        private async Task<string> ResolvePnSubjectAsync(IReadOnlyDictionary<string, string?> keyValues) =>
            await ResolveSingleKeySubjectAsync(keyValues, "PNID", ResolvePnNameAsync);

        private async Task<string> ResolveResidentStatusSubjectAsync(IReadOnlyDictionary<string, string?> keyValues) =>
            await ResolveSingleKeySubjectAsync(keyValues, "ResidentStatusID", ResolveResidentStatusNameAsync);

        private async Task<string> ResolveSpecialResponsibilitySubjectAsync(IReadOnlyDictionary<string, string?> keyValues) =>
            await ResolveSingleKeySubjectAsync(keyValues, "SpecialResponsibilityID", ResolveSpecialResponsibilityNameAsync);

        private async Task<string> ResolveDepartmentSubjectAsync(IReadOnlyDictionary<string, string?> keyValues) =>
            await ResolveSingleKeySubjectAsync(keyValues, "DepartmentID", ResolveDepartmentNameAsync);

        private async Task<string> ResolveDepartmentTaskSubjectAsync(IReadOnlyDictionary<string, string?> keyValues) =>
            await ResolveSingleKeySubjectAsync(keyValues, "DepartmentTaskID", ResolveDepartmentTaskNameAsync);

        private async Task<string> ResolveGroceryDaySubjectAsync(IReadOnlyDictionary<string, string?> keyValues) =>
            await ResolveSingleKeySubjectAsync(keyValues, "GroceryDayID", ResolveGroceryDayNameAsync);

        private async Task<string> ResolvePaymentMethodSubjectAsync(IReadOnlyDictionary<string, string?> keyValues) =>
            await ResolveSingleKeySubjectAsync(keyValues, "PaymentMethodID", ResolvePaymentMethodNameAsync);

        private async Task<string> ResolvePhoneSubjectAsync(IReadOnlyDictionary<string, string?> keyValues) =>
            await ResolveSingleKeySubjectAsync(keyValues, "PhoneID", ResolvePhoneNameAsync);

        private async Task<string> ResolveRiskLevelSubjectAsync(IReadOnlyDictionary<string, string?> keyValues) =>
            await ResolveSingleKeySubjectAsync(keyValues, "RiskLevelID", ResolveRiskLevelNameAsync);

        private async Task<string> ResolveStaffResidentStatusSubjectAsync(IReadOnlyDictionary<string, string?> keyValues) {
            var staffName = await ResolveKeyValueAsync(keyValues, "StaffID", ResolveStaffNameAsync);
            var residentStatusName = await ResolveKeyValueAsync(keyValues, "ResidentStatusID", ResolveResidentStatusNameAsync);

            return JoinSubjectParts(staffName, residentStatusName);
        }

        private async Task<string> ResolveResidentPaymentMethodSubjectAsync(IReadOnlyDictionary<string, string?> keyValues) {
            var residentName = await ResolveKeyValueAsync(keyValues, "ResidentID", ResolveResidentNameAsync);
            var paymentMethodName = await ResolveKeyValueAsync(keyValues, "PaymentMethodID", ResolvePaymentMethodNameAsync);

            return JoinSubjectParts(residentName, paymentMethodName);
        }

        private async Task<string> ResolveSpecialResponsibilityStaffSubjectAsync(IReadOnlyDictionary<string, string?> keyValues) {
            var responsibilityName = await ResolveKeyValueAsync(keyValues, "SpecialResponsibilityID", ResolveSpecialResponsibilityNameAsync);
            var staffName = await ResolveKeyValueAsync(keyValues, "StaffID", ResolveStaffNameAsync);

            return JoinSubjectParts(responsibilityName, staffName);
        }

        private async Task<string> ResolveFallbackSubjectAsync(IReadOnlyDictionary<string, string?> keyValues) {
            var resolvedParts = new List<string>();

            foreach (var keyValue in keyValues) {
                resolvedParts.Add(await ResolveValueAsync(keyValue.Key, keyValue.Value) ?? " ");
            }

            return JoinSubjectParts(resolvedParts.ToArray());
        }

        private async Task<string> ResolveSingleKeySubjectAsync(
            IReadOnlyDictionary<string, string?> keyValues,
            string key,
            Func<Guid, Task<string?>> resolver) {
            var resolvedValue = await ResolveKeyValueAsync(keyValues, key, resolver);
            return string.IsNullOrWhiteSpace(resolvedValue)
                ? await ResolveFallbackSubjectAsync(keyValues)
                : resolvedValue;
        }

        private static async Task<string?> ResolveKeyValueAsync(
            IReadOnlyDictionary<string, string?> keyValues,
            string key,
            Func<Guid, Task<string?>> resolver) {
            return keyValues.TryGetValue(key, out var value) && Guid.TryParse(value, out var id)
                ? await resolver(id)
                : null;
        }

        private async Task<string?> ResolveShiftBoardNameAsync(Guid id) {
            var shiftBoard = await _context.ShiftBoards
                .AsNoTracking()
                .Where(shiftBoard => shiftBoard.ShiftBoardID == id)
                .Select(shiftBoard => new {
                    shiftBoard.ShiftType,
                    shiftBoard.StartDateTime,
                    shiftBoard.EndDateTime
                })
                .FirstOrDefaultAsync();

            return shiftBoard is null
                ? null
                : $"{shiftBoard.ShiftType} {shiftBoard.StartDateTime:dd-MM-yyyy HH:mm}";
        }

        private async Task<string?> ResolveStaffNameAsync(Guid id) =>
            await _context.Staffs
                .AsNoTracking()
                .Where(staff => staff.StaffID == id)
                .Select(staff => staff.StaffName)
                .FirstOrDefaultAsync();

        private async Task<string?> ResolveResidentNameAsync(Guid id) =>
            await _context.Residents
                .AsNoTracking()
                .Where(resident => resident.ResidentID == id)
                .Select(resident => resident.ResidentName)
                .FirstOrDefaultAsync();

        private async Task<string?> ResolveMedicineNameAsync(Guid id) {
            var medicine = await _context.Medicines
                .AsNoTracking()
                .Where(medicine => medicine.MedicineID == id)
                .Select(medicine => new {
                    medicine.Resident.ResidentName,
                    medicine.ScheduledTime
                })
                .FirstOrDefaultAsync();

            return medicine is null
                ? null
                : $"{medicine.ResidentName} - medicin {medicine.ScheduledTime:HH:mm}";
        }

        private async Task<string?> ResolvePnNameAsync(Guid id) {
            var pn = await _context.PNs
                .AsNoTracking()
                .Where(pn => pn.PNID == id)
                .Select(pn => new {
                    pn.Resident.ResidentName,
                    pn.PNGivenTime
                })
                .FirstOrDefaultAsync();

            return pn is null
                ? null
                : $"{pn.ResidentName} - PN {pn.PNGivenTime:dd-MM-yyyy HH:mm}";
        }

        private async Task<string?> ResolveResidentStatusNameAsync(Guid id) {
            var residentStatus = await _context.ResidentStatuses
                .AsNoTracking()
                .Where(status => status.ResidentStatusID == id)
                .Select(status => new {
                    status.Resident.ResidentName,
                    status.Date
                })
                .FirstOrDefaultAsync();

            return residentStatus is null
                ? null
                : $"{residentStatus.ResidentName} - status {residentStatus.Date:dd-MM-yyyy HH:mm}";
        }

        private async Task<string?> ResolveSpecialResponsibilityNameAsync(Guid id) =>
            await _context.SpecialResponsibilities
                .AsNoTracking()
                .Where(responsibility => responsibility.SpecialResponsibilityID == id)
                .Select(responsibility => responsibility.TaskName)
                .FirstOrDefaultAsync();

        private async Task<string?> ResolveDepartmentNameAsync(Guid id) =>
            await _context.Departments
                .AsNoTracking()
                .Where(department => department.DepartmentID == id)
                .Select(department => department.DepartmentName)
                .FirstOrDefaultAsync();

        private async Task<string?> ResolveDepartmentTaskNameAsync(Guid id) =>
            await _context.DepartmentTasks
                .AsNoTracking()
                .Where(task => task.DepartmentTaskID == id)
                .Select(task => task.DepartmentTaskName)
                .FirstOrDefaultAsync();

        private async Task<string?> ResolveGroceryDayNameAsync(Guid id) =>
            await _context.GroceryDays
                .AsNoTracking()
                .Where(groceryDay => groceryDay.GroceryDayID == id)
                .Select(groceryDay => groceryDay.GroceryDayName)
                .FirstOrDefaultAsync();

        private async Task<string?> ResolvePaymentMethodNameAsync(Guid id) =>
            await _context.PaymentMethods
                .AsNoTracking()
                .Where(paymentMethod => paymentMethod.PaymentMethodID == id)
                .Select(paymentMethod => paymentMethod.PaymentMethodName)
                .FirstOrDefaultAsync();

        private async Task<string?> ResolvePhoneNameAsync(Guid id) =>
            await _context.Phones
                .AsNoTracking()
                .Where(phone => phone.PhoneID == id)
                .Select(phone => phone.PhoneNumber)
                .FirstOrDefaultAsync();

        private async Task<string?> ResolveRiskLevelNameAsync(Guid id) =>
            await _context.RiskLevels
                .AsNoTracking()
                .Where(riskLevel => riskLevel.RiskLevelID == id)
                .Select(riskLevel => riskLevel.RiskLevelName)
                .FirstOrDefaultAsync();

        private static Dictionary<string, string?> ParseAuditValues(string? json) {
            var values = new Dictionary<string, string?>();

            if (string.IsNullOrWhiteSpace(json)) {
                return values;
            }

            try {
                using var document = JsonDocument.Parse(json);
                if (document.RootElement.ValueKind != JsonValueKind.Object) {
                    return values;
                }

                foreach (var property in document.RootElement.EnumerateObject()) {
                    values[property.Name] = FormatJsonValue(property.Value);
                }
            }
            catch (JsonException) {
                return values;
            }

            return values;
        }

        private static string? FormatJsonValue(JsonElement value) {
            return value.ValueKind switch {
                JsonValueKind.Null => null,
                JsonValueKind.String => value.GetString(),
                JsonValueKind.True => "Ja",
                JsonValueKind.False => "Nej",
                JsonValueKind.Number => value.GetRawText(),
                _ => value.GetRawText()
            };
        }

        private static string JoinSubjectParts(params string?[] parts) {
            var subject = string.Join(", ", parts.Where(part => !string.IsNullOrWhiteSpace(part)));
            return string.IsNullOrWhiteSpace(subject) ? "-" : subject;
        }

        private static string GetUnknownReferenceText(string propertyName) {
            var displayName = propertyName.EndsWith("ID", StringComparison.Ordinal)
                ? propertyName[..^2]
                : propertyName;

            return $"Ukendt {SplitPascalCase(displayName).ToLowerInvariant()}";
        }

        private static string SplitPascalCase(string value) {
            if (string.IsNullOrWhiteSpace(value)) {
                return string.Empty;
            }

            return string.Concat(value.Select((character, index) =>
                index > 0 && char.IsUpper(character) && !char.IsUpper(value[index - 1])
                    ? $" {character}"
                    : character.ToString()));
        }
    }
}
