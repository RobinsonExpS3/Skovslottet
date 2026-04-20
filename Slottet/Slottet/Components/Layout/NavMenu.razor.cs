namespace Slottet.Components.Layout {
    public partial class NavMenu {
        private bool isAdminOpen;

        private void ToggleAdminMenu() {
            isAdminOpen = !isAdminOpen;
        }
    }
}