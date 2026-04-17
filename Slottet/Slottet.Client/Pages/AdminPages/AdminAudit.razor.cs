namespace Slottet.Client.Pages.AdminPages
{
    public partial class AdminAudit
    {

        public class ShiftBoard()
        {
        }

        List<ShiftBoard> shiftBoards = new();

        private bool isStatusOpen;
        int? openCardId = null;

        //private void ToggleStatusCard() {
        //    isStatusOpen = !isStatusOpen;
        //}

        private void ToggleStatusCard()
        {
            Console.WriteLine("CLICKED");
            isStatusOpen = !isStatusOpen;
        }
    }
}