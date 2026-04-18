using Slottet.Client.Pages.MockData;
using Slottet.Shared;

namespace Slottet.Client.Pages.AdminPages
{
    public partial class AdminAudit
    {
        protected ShiftBoardDTO? Model { get; set; }

        private Guid? openCardId = null;

        protected override void OnInitialized() {
            Model = ShiftBoardMockData.Create();
        }

        private void ToggleStatusCard(Guid residentCardId)
        {
            openCardId = openCardId == residentCardId ? null : residentCardId;
        }
    }
}