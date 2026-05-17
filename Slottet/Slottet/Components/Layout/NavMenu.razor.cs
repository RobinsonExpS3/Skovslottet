using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;

namespace Slottet.Components.Layout {
    public partial class NavMenu {
        private bool isAdminOpen;
        private bool canSeeAdminMenu;

        [Inject]
        private HttpClient Http { get; set; } = default!;

        protected override async Task OnInitializedAsync() {
            try {
                var user = await Http.GetFromJsonAsync<CurrentUser>("api/Auth/me");
                canSeeAdminMenu = user?.Roles.Any(r =>
                    r.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                    r.Equals("Developer", StringComparison.OrdinalIgnoreCase)) == true;
            } catch {
                canSeeAdminMenu = false;
            }
            StateHasChanged();
        }

        private void ToggleAdminMenu() {
            isAdminOpen = !isAdminOpen;
        }

        private sealed record CurrentUser(string? Name, string[] Roles);
    }
}
