using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Slottet.Shared;

namespace Slottet.Client.Pages.AdminPages
{
    public partial class AdminAudit
    {
        [Inject]
        private HttpClient httpClient { get; set; } = default!;

        protected List<AuditLogDTO> AuditRows { get; set; } = new();
        private List<AuditDisplayRow> _allDisplayRows = new();
        protected IReadOnlyList<AuditGroup> FilteredGroups { get; private set; } = new List<AuditGroup>();
        protected List<ResidentCardDto> ResidentCards { get; set; } = new();
        protected DateOnly SelectedDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        protected string SelectedShift { get; set; } = string.Empty;

        protected bool IsLoading { get; set; }
        protected string? ErrorMessage { get; set; }

        protected bool IsResidentLoading { get; set; }
        protected string? ResidentErrorMessage { get; set; }

        protected string SelectedDateText => SelectedDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        private Guid? openCardId = null;
        private CancellationTokenSource? _searchCts;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                using var response = await httpClient.GetAsync("api/shiftboard/current");

                if (response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized)
                {
                    ErrorMessage = "Du har ikke adgang til denne side.";
                    return;
                }

                if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = "Kunne ikke loade vagttavlen. Prøv venligst igen senere.";
                    return;
                }

                //Model = await response.Content.ReadFromJsonAsync<ShiftBoardDTO>();
            }
            catch
            {
                ErrorMessage = "Kunne ikke loade vagttavlen. Prøv venligst igen senere.";
            }
            finally
            {
                IsLoading = false;
                await LoadAuditLogsAsync();
                await LoadResidentCardsAsync();
            }

        }

        private async Task LoadAuditLogsAsync()
        {
            IsLoading = true;
            ErrorMessage = null;

            try
            {
                var query = new List<string> {
                    $"date={SelectedDate:yyyy-MM-dd}"
                };

                if (!string.IsNullOrWhiteSpace(SelectedShift))
                {
                    query.Add($"shift={Uri.EscapeDataString(SelectedShift)}");
                }

                var url = $"api/AuditLog?{string.Join("&", query)}";
                var result = await AdminHttp.GetJsonAsync<List<AuditLogDTO>>(httpClient, url);
                if (result.Failed)
                {
                    ErrorMessage = result.ErrorMessage;
                    AuditRows = new();
                    return;
                }

                AuditRows = result.Value
                    .OrderByDescending(r => r.PerformedAtTime)
                    .ToList();

                _allDisplayRows = AuditRows.SelectMany(CreateAuditDisplayRows).ToList();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Kunne ikke hente audit logs: {ex.Message}";
                AuditRows = new();
                _allDisplayRows = new();
                FilteredGroups = new List<AuditGroup>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Subjects der altid grupperes uanset vagttavle
        private static readonly HashSet<string> _fixedGroupSubjects = new(StringComparer.OrdinalIgnoreCase)
        {
            "Telefoner", "Særligt Ansvar", "Handledage", "Afdelinger", "Afdelingsopgaver", "Betalingsmetoder"
        };

        private void ApplyFilter()
        {
            var rows = string.IsNullOrWhiteSpace(SearchText)
                ? _allDisplayRows
                : _allDisplayRows.Where(r => MatchesSearch(r, SearchText)).ToList();

            FilteredGroups = rows
                .GroupBy(r =>
                {
                    var baseName = ExtractBaseName(r.Subject);
                    var shiftKey = _fixedGroupSubjects.Contains(baseName) ? "–" : r.ShiftBoard;
                    return (baseName, shiftKey);
                })
                .OrderByDescending(g => g.Max(r => r.Time))
                .Select(g => new AuditGroup(
                    g.Key.baseName,
                    _fixedGroupSubjects.Contains(g.Key.baseName) ? "–" : g.Key.shiftKey,
                    string.Join(", ", g.Select(r => r.PerformedBy).Distinct()),
                    g.Max(r => r.Time),
                    g.OrderByDescending(r => r.Time).ToList()))
                .ToList();
        }

        protected static string ExtractBaseName(string subject)
        {
            var idx = subject.IndexOf(" - ", StringComparison.Ordinal);
            return idx >= 0 ? subject[..idx] : subject;
        }

        protected static string ExtractDetail(string subject, string baseName)
        {
            if (subject.Length <= baseName.Length + 3) return string.Empty;
            return subject.StartsWith(baseName + " - ", StringComparison.Ordinal)
                ? subject[(baseName.Length + 3)..]
                : string.Empty;
        }

        private async Task LoadResidentCardsAsync()
        {
            IsResidentLoading = true;
            ResidentErrorMessage = null;

            try
            {
                var dto = await httpClient.GetFromJsonAsync<ShiftBoardDTO>("api/shiftboard/current");
                ResidentCards = dto?.ResidentCards ?? new();
            }
            catch (Exception ex)
            {
                ResidentErrorMessage = $"Kunne ikke hente beboere: {ex.Message}";
            }
            finally
            {
                IsResidentLoading = false;
            }
        }

        private void ToggleStatusCard(Guid residentCardId)
        {
            openCardId = openCardId == residentCardId ? null : residentCardId;
        }

        protected async Task OnDateChanged(ChangeEventArgs args)
        {
            var dateText = args.Value?.ToString();

            if (DateOnly.TryParseExact(
                dateText,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsedDate))
            {
                SelectedDate = parsedDate;
                await LoadAuditLogsAsync();
            }
        }

        protected async Task OnShiftChanged(ChangeEventArgs args)
        {
            SelectedShift = args.Value?.ToString() ?? string.Empty;
            await LoadAuditLogsAsync();
        }

        private static IEnumerable<AuditDisplayRow> CreateAuditDisplayRows(AuditLogDTO row)
        {
            var keyValues = GetAuditValues(row.KeyValues);
            var oldValues = GetAuditValues(row.OldValuesJson);
            var newValues = GetAuditValues(row.NewValuesJson);
            var changedFields = oldValues.Keys
                .Union(newValues.Keys)
                .Where(field => !string.IsNullOrWhiteSpace(field))
                .ToList();

            // Join tables (e.g. StaffResidentStatuses) have only PK columns — interceptor skips those,
            // so old/new values are empty. Use a sentinel so we still emit one display row.
            var noValueFields = changedFields.Count == 0;
            if (noValueFields)
            {
                changedFields.Add(string.Empty);
            }

            var action = string.IsNullOrWhiteSpace(row.DisplayAction)
                ? FormatAction(row.Action)
                : row.DisplayAction;

            foreach (var field in changedFields)
            {
                yield return new AuditDisplayRow(
                    row.PerformedAtTime,
                    row.PerformedByStaffName,
                    action,
                    row.Category,
                    string.IsNullOrWhiteSpace(row.ShiftBoardLabel) ? "–" : row.ShiftBoardLabel,
                    string.IsNullOrWhiteSpace(row.Subject) ? "–" : row.Subject,
                    noValueFields ? "–" : TranslatePropertyName(field),
                    noValueFields ? "–" : oldValues.GetValueOrDefault(field, "–"),
                    noValueFields ? "–" : newValues.GetValueOrDefault(field, "–"));
            }
        }

        protected static IReadOnlyDictionary<string, string> GetAuditValues(string? json)
        {
            var values = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(json))
            {
                return values;
            }

            try
            {
                using var document = JsonDocument.Parse(json);

                if (document.RootElement.ValueKind != JsonValueKind.Object)
                {
                    values[string.Empty] = FormatJsonValue(document.RootElement);
                    return values;
                }

                foreach (var property in document.RootElement.EnumerateObject())
                {
                    values[property.Name] = FormatJsonValue(property.Value);
                }

                return values;
            }
            catch (JsonException)
            {
                values[string.Empty] = json;
                return values;
            }
        }

        private static string FormatAction(string action)
        {
            return action switch
            {
                "Added" => "Oprettede",
                "Modified" => "Ændrede",
                "Deleted" => "Arkiverede",
                _ => action
            };
        }

        private static string TranslatePropertyName(string name) {
            return name switch {
                "IsGiven"              => "Givet",
                "GivenTime"            => "Givet kl.",
                "MedicinStatus"        => "",
                "PNDetaljer"           => "",
                "AnsvarligtPersonale"  => "Ansvarligt personale",
                "RegistreretAf"        => "",
                "StaffName"        => "Navn",
                "Initials"         => "Initialer",
                "Role"             => "Rolle",
                "DepartmentID"     => "Afdeling",
                "StatusNote"       => "Status",
                "RiskLevelID"      => "Risiko",
                "PaymentMethodID"  => "Betalingsmetode",
                "GroceryDayID"     => "Handledag",
                "MedicationName"   => "Medicin",
                "ScheduledTime"    => "Tidspunkt",
                "ResidentName"     => "Navn",
                "PhoneNumber"      => "Telefonnummer",
                "TaskName"         => "Opgave",
                "IsActive"         => "Aktiv",
                "Description"      => "Beskrivelse",
                "ShiftType"        => "Vagttype",
                "StartDateTime"    => "Starttidspunkt",
                "EndDateTime"      => "Sluttidspunkt",
                "DepartmentName"   => "Afdelingsnavn",
                "ResidentID"       => "Beboer",
                "StaffID"          => "Medarbejder",
                "SortOrder"        => "Sortering",
                "AssignedAt"       => "Tildelt den",
                "Date"             => "Dato",
                "Status"           => "",
                "PNGivenTime"      => "PN tidspunkt",
                "Medication"       => "Medicin",
                "Reason"           => "Årsag",
                "IssuedBy"         => "Ordineret af",
                _                  => FormatPropertyName(name)
            };
        }

        private static string FormatPropertyName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return string.Empty;
            }

            var cleanedName = name.EndsWith("ID", StringComparison.Ordinal)
                ? name[..^2]
                : name;

            return string.Concat(cleanedName.Select((character, index) =>
                index > 0 && char.IsUpper(character) && !char.IsUpper(cleanedName[index - 1])
                    ? $" {character}"
                    : character.ToString()));
        }

        private static string FormatJsonValue(JsonElement value)
        {
            if (value.ValueKind == JsonValueKind.String)
            {
                var str = value.GetString();
                if (str is null) return "–";
                if (DateTimeOffset.TryParse(str, out var dto))
                    return dto.ToString("dd-MM-yyyy HH:mm");
                return str;
            }

            return value.ValueKind switch
            {
                JsonValueKind.Null  => "–",
                JsonValueKind.True  => "Ja",
                JsonValueKind.False => "Nej",
                JsonValueKind.Number => value.GetRawText(),
                _ => value.GetRawText()
            };
        }

        protected sealed record AuditDisplayRow(
            DateTime Time,
            string PerformedBy,
            string Action,
            string Category,
            string ShiftBoard,
            string Subject,
            string Field,
            string OldValue,
            string NewValue);

        protected sealed record AuditGroup(
            string Subject,
            string ShiftBoard,
            string Performers,
            DateTime LatestTime,
            IReadOnlyList<AuditDisplayRow> Rows);

        protected static string GetActionCssClass(string action) => action switch
        {
            var a when a.StartsWith("Gav", StringComparison.OrdinalIgnoreCase)
                    || a.StartsWith("Tildel", StringComparison.OrdinalIgnoreCase)
                    || a.StartsWith("Oprette", StringComparison.OrdinalIgnoreCase)
                    || a.StartsWith("Tilføje", StringComparison.OrdinalIgnoreCase)
                    || a.StartsWith("Registre", StringComparison.OrdinalIgnoreCase) => "audit-action-add",
            var a when a.StartsWith("Ændre", StringComparison.OrdinalIgnoreCase) => "audit-action-modify",
            _ => "audit-action-remove"
        };

        protected string SearchText { get; set; } = string.Empty;

        protected async Task OnSearchInput(ChangeEventArgs e)
        {
            SearchText = e.Value?.ToString() ?? string.Empty;

            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            try
            {
                await Task.Delay(300, token);
                ApplyFilter();
            }
            catch (OperationCanceledException) { }
        }

        private static bool MatchesSearch(AuditDisplayRow row, string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText)) return true;

            return Contains(row.PerformedBy, searchText)
                || Contains(row.Action, searchText)
                || Contains(row.Category, searchText)
                || Contains(row.ShiftBoard, searchText)
                || Contains(row.Subject, searchText)
                || Contains(row.Field, searchText)
                || Contains(row.OldValue, searchText)
                || Contains(row.NewValue, searchText);
        }

        private static bool Contains(string value, string searchText)
        {
            return value.Contains(searchText, StringComparison.OrdinalIgnoreCase);
        }
    }
}
