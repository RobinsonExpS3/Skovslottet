namespace Slottet.Shared
{
    public sealed class ResidentCardViewDto //For at indlæse alt nødvendige data til vores residentcards i udviklingsfasen. :) 
    {                                       //Kan erstattes med modellerne, men så skal vi hente data fra flere modellag,  
                                            //istedet for at have en klasse med et ansvar.
        public Guid ResidentCardId { get; set; }
        public Guid ResidentId { get; set; }
        public DateTime Date { get; set; }

        public string ResidentName { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public string? RiskLevel { get; set; }
        public string? LatestStatusNote { get; set; }

        public List<MedicineScheduleItemDto> MedicineSchedule { get; set; } = new();
        public string? PNStatus { get; set; }
        public List<string> AssignedStaff { get; set; } = new();


        public string? GroceryDay { get; set; }
        public string? PaymentMethod { get; set; }

        public string RiskCssClass => RiskLevel switch
        {
            "Rød" => "risk-red",
            "Gul" => "risk-yellow",
            "Grøn" => "risk-green",
            _ => "risk-default"
        };
    }
}
