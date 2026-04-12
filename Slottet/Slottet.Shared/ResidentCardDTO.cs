using System.Security.Cryptography.X509Certificates;

namespace Slottet.Shared
{
    public sealed class ResidentCardDto //For at indlæse alt nødvendige data til vores residentcards i udviklingsfasen. :) 
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

    //Opdateret DTO - Vi skal nok have medicine i en DTO, der læses ind her ligesom i mock data
    public sealed class ResidentCardDTO
    {
        // Resident Card 
        public string Status { get; set; }
        public DateTime StatusEntryTime { get; set; }

        // Resident 
        public string ResidentName { get; set; }
        public bool IsActive { get; set; }

        // Risk 
        public string RiskLevelName { get; set; }

        //PN
        public string PNSatus { get; set; }

        //Medicine
        public List<MedicineSlotDTO> Medicines { get; set; }

        //Grocery Day
        public string GroceryDayName { get; set; }

        //Payment Method
        public List<string> PaymentMethods { get; set; }

        //staff 
        public List<string> ResponsibleStaff { get; set; } // (?)


    }
}
