using System.Globalization;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Slottet.Shared;

namespace Slottet.Client.Pages.AdminPages
{
    public partial class AdminAudit
    {
        [Inject]
        private HttpClient httpClient { get; set; } = default!;

        protected List<AuditLogDTO> AuditRows { get; set; } = new();
        protected List<ResidentCardDto> ResidentCards { get; set; } = new();
        protected DateOnly SelectedDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        protected string SelectedShift { get; set; } = string.Empty;

        protected bool IsLoading { get; set; }
        protected string? ErrorMessage { get; set; }

        protected bool IsResidentLoading { get; set; }
        protected string? ResidentErrorMessage { get; set; }

        protected string SelectedDateText => SelectedDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        private Guid? openCardId = null;

        protected override async Task OnInitializedAsync() {
            await LoadAuditLogsAsync();
            await LoadResidentCardsAsync();
        }

        private async Task LoadAuditLogsAsync() {
            IsLoading = true;
            ErrorMessage = null;

            try {
                var query = new List<string> {
                    $"date={SelectedDate:yyyy-MM-dd}"
                };

                if (!string.IsNullOrWhiteSpace(SelectedShift)) {
                    query.Add($"shift={Uri.EscapeDataString(SelectedShift)}");
                }

                var url = $"api/AuditLog?{string.Join("&", query)}";
                var result = await AdminHttp.GetJsonAsync<List<AuditLogDTO>>(httpClient, url);
                if (result.Failed) {
                    ErrorMessage = result.ErrorMessage;
                    AuditRows = new();
                    return;
                }

                AuditRows = (result.Value ?? new())
                    .OrderByDescending(r => r.PerformedAtTime)
                    .ToList();
            } catch {
                ErrorMessage = "Kunne ikke hente audit logs.";
                AuditRows = new();
            }
            finally {
                IsLoading = false;
            }
        }

        private async Task LoadResidentCardsAsync() {
            IsResidentLoading = true;
            ResidentErrorMessage = null;

            try {
                var result = await AdminHttp.GetJsonAsync<ShiftBoardDTO>(httpClient, "api/shiftboard/current");
                if (result.Failed) {
                    ResidentErrorMessage = result.ErrorMessage;
                    ResidentCards = new();
                    return;
                }

                var dto = result.Value;
                ResidentCards = dto?.ResidentCards ?? new();
            } catch {
                ResidentErrorMessage = "Kunne ikke hente beboere.";
            } finally {
                IsResidentLoading = false;
            }
        }

        private void ToggleStatusCard(Guid residentCardId)
        {
            openCardId = openCardId == residentCardId ? null : residentCardId;
        }

        protected async Task OnDateChanged(ChangeEventArgs args) {
            var dateText = args.Value?.ToString();

            if (DateOnly.TryParseExact(
                dateText,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsedDate)) {
                SelectedDate = parsedDate;
                await LoadAuditLogsAsync();
            }
        }

        protected async Task OnShiftChanged(ChangeEventArgs args) {
            SelectedShift = args.Value?.ToString() ?? string.Empty;
            await LoadAuditLogsAsync();
        }
    }
}
