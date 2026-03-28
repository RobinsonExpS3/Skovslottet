using Slottet.Shared;

namespace Slottet.Client.Pages.MockData
{
    public static class ShiftBoardMockData
    {
        public static ShiftBoardViewDto Create()
        {
            return new ShiftBoardViewDto
            {
                ShiftBoardId = Guid.NewGuid(),
                ShiftType = "Dagvagt",
                StartDate = DateTime.Today.AddHours(7),
                EndDate = DateTime.Today.AddHours(15),
                DepartmentName = "Skoven",

                PhoneNumbers = new List<string>
            {
                "12 34 56 78",
                "87 65 43 21"
            },

                DepartmentTasks = new List<string>
            {
                "Sprit af",
                "Lav Kaffe",
                "Tøm skraldespande"
            },

                SpecialResponsibilities = new List<string>
            {
                "Medicinansvarlig",
                "Hovedtelefon"
            },

                ResidentCards = new List<ResidentCardViewDto>
            {
                new()
                {
                    ResidentCardId = Guid.NewGuid(),
                    ResidentId = Guid.NewGuid(),

                    ResidentName = "Anna Bentsen",
                    IsActive = true,

                    RiskLevel = "Lav",
                    Status = "Stabil",
                    
                    
                    PnTime = "20:00",
                    MedicinTime = "10:00",

                    GroceryDay = DateTime.Today.AddDays(1),
                    PaymentMethod = "Dankort"
                },
                new()
                {
                    ResidentCardId = Guid.NewGuid(),
                    ResidentId = Guid.NewGuid(),

                    ResidentName = "Carsten Didriksen",
                    IsActive = true,

                    RiskLevel = "Alt godt",
                    Status = "Grøn",


                    PnTime = "20:00",
                    MedicinTime = "07:00",

                    GroceryDay = DateTime.Today.AddDays(1),
                    PaymentMethod = "P-kort"
                },
                new()
                {
                    ResidentCardId = Guid.NewGuid(),
                    ResidentId = Guid.NewGuid(),

                    ResidentName = "Enaya Frederiksen",
                    IsActive = true,

                    RiskLevel = "Rød",
                    Status = "Ked af det",


                    PnTime = "20:00",
                    MedicinTime = "22:00",

                    GroceryDay = DateTime.Today.AddDays(1),
                    PaymentMethod = "Mobilepay"
                }
            }
            };
        }
    }
}
