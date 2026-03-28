namespace Slottet.Shared
{
    public sealed class ResidentCardViewDto //For at indlæse alt nødvendige data til vores residentcards i udviklingsfasen. :) 
    {                                       //Kan erstattes med modellerne, men så skal vi hente data fra flere modellag,  
                                            //istedet for at have en klasse med et ansvar.
        public Guid ResidentCardId { get; set; }
        public Guid ResidentId { get; set; }

        public string ResidentName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? GroceryDay { get; set; }

        public string Status { get; set; } = string.Empty;
        public DateTime Date { get; set; }

        public string? RiskLevel { get; set; }
        public string? PnTime { get; set; }
    }
}
