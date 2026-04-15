using Slottet.Domain.Entities;

namespace Slottet.Client.Pages.AdminPages {
    public partial class AdminAudit {
        List<ShiftBoard> shiftBoards = new();

        private bool isStatusOpen;
        int? openCardId = null;

        //private void ToggleStatusCard() {
        //    isStatusOpen = !isStatusOpen;
        //}

        private void ToggleStatusCard() {
            Console.WriteLine("CLICKED");
            isStatusOpen = !isStatusOpen;
        }
    }
}