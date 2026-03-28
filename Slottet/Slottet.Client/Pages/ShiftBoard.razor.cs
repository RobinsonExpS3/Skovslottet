using Slottet.Client.Pages.MockData;
using Slottet.Shared;

namespace Slottet.Client.Pages
{
    public partial class ShiftBoard //Midlertidig codebehind. Skal lige prøve det af. :D
    {
        protected ShiftBoardViewDto? Model { get; set; }

        protected override void OnInitialized()
        {
            Model = ShiftBoardMockData.Create();
        }

        //private List<ResidentCardDTO> residentCards = new List<ResidentCardDTO>();
    }
}