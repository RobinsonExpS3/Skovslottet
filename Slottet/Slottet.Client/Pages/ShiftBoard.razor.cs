using Slottet.Client.Pages.MockData;
using Slottet.Shared;

namespace Slottet.Client.Pages
{
    /// <summary>
    /// Represents the main component for displaying and managing the shift board view in the application.
    /// </summary>
    /// 
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