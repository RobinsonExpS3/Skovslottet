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
        protected IReadOnlyList<AuditDisplayRow> AuditDisplayRows => AuditRows
            .SelectMany(CreateAuditDisplayRows)
            .ToList();
        protected List<ResidentCardDto> ResidentCards { get; set; } = new();
        protected DateOnly SelectedDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        protected string SelectedShift { get; set; } = string.Empty;

        protected bool IsLoading { get; set; }
        protected string? ErrorMessage { get; set; }

        protected bool IsResidentLoading { get; set; }
        protected string? ResidentErrorMessage { get; set; }

        protected string SelectedDateText => SelectedDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        private Guid? openCardId = null;

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
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Kunne ikke hente audit logs: {ex.Message}";
                AuditRows = new();
            }
            finally
            {
                IsLoading = false;
            }
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

            if (changedFields.Count == 0)
            {
                changedFields.Add(string.Empty);
            }

            foreach (var field in changedFields)
            {
                yield return new AuditDisplayRow(
                    row.PerformedAtTime,
                    row.PerformedByStaffName,
                    FormatAction(row.Action),
                    row.TableName,
                    string.IsNullOrWhiteSpace(row.Subject) ? "Ikke givet" : row.Subject,
                    oldValues.GetValueOrDefault(field, "Ikke givet"),
                    newValues.GetValueOrDefault(field, "Ikke givet"));
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
            return value.ValueKind switch
            {
                JsonValueKind.Null => "Ikke givet",
                JsonValueKind.String => value.GetString() ?? "Ikke givet",
                JsonValueKind.True => "Ja",
                JsonValueKind.False => "Nej",
                JsonValueKind.Number => value.GetRawText(),
                _ => value.GetRawText()
            };
        }

        protected sealed record AuditDisplayRow(
            DateTime Time,
            string PerformedBy,
            string Action,
            string TableName,
            string Subject,
            string OldValue,
            string NewValue);

        protected string SearchText { get; set; } = string.Empty;

        protected IReadOnlyList<AuditDisplayRow> FilteredAuditDisplayRows =>
            AuditDisplayRows
                .Where(row => MatchesSearch(row, SearchText))
                .ToList();

        private static bool MatchesSearch(AuditDisplayRow row, string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText)) return true;

            return Contains(row.PerformedBy, searchText)
                || Contains(row.Action, searchText)
                || Contains(row.TableName, searchText)
                || Contains(row.Subject, searchText)
                || Contains(row.OldValue, searchText)
                || Contains(row.NewValue, searchText);
        }

        private static bool Contains(string value, string searchText)
        {
            return value.Contains(searchText, StringComparison.OrdinalIgnoreCase);
        }
    }
}
