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
                "41522",
                "41523",
                "41524",
                "41525",
                "41526",
                "41527",
                "41528",
                "41529"

            },

                DepartmentTasks = new List<string>
            {
                    "Tjek borger kalender",
                    "Tjek FMK, Sundhedsplaner og delmål og en massse andre skørre aaarrh"
            },

                SpecialResponsibilities = new List<string>
            {
                "Medicinansvarlig",
                "Hovedtelefon",
                "Sprit af",
                "Lav Kaffe",
                "Tøm skraldespande"
            },

                ResidentCards = new List<ResidentCardViewDto>
            {
                new()
                {
                    ResidentCardId = Guid.NewGuid(),
                    ResidentId = Guid.NewGuid(),
                    Date = DateTime.Today,

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
                    Date = DateTime.Today,

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
                    Date = DateTime.Today,

                    ResidentName = "Enaya Frederiksen",
                    IsActive = true,

                    RiskLevel = "Rød",
                    Status = "Ked af det",


                    PnTime = "20:00",
                    MedicinTime = "22:00",

                    GroceryDay = DateTime.Today.AddDays(1),
                    PaymentMethod = "Mobilepay"
                },
                new()
                {
                    ResidentCardId = Guid.NewGuid(),
                    ResidentId = Guid.NewGuid(),
                    Date = DateTime.Today,

                    ResidentName = "Gert Heller",
                    IsActive = true,

                    RiskLevel = "Rød",
                    Status = "Ked af det",


                    PnTime = "20:00",
                    MedicinTime = "22:00",

                    GroceryDay = DateTime.Today.AddDays(1),
                    PaymentMethod = "Mobilepay"
                },
                new()
                {
                    ResidentCardId = Guid.NewGuid(),
                    ResidentId = Guid.NewGuid(),
                    Date = DateTime.Today,

                    ResidentName = "Ida Jacoby",
                    IsActive = true,

                    RiskLevel = "Rød",
                    Status = "Ked af det",


                    PnTime = "20:00",
                    MedicinTime = "22:00",

                    GroceryDay = DateTime.Today.AddDays(1),
                    PaymentMethod = "Mobilepay"
                },
                new()
                {
                    ResidentCardId = Guid.NewGuid(),
                    ResidentId = Guid.NewGuid(),
                    Date = DateTime.Today,

                    ResidentName = "Karl Larsen",
                    IsActive = true,

                    RiskLevel = "Rød",
                    Status = "Ked af det",


                    PnTime = "20:00",
                    MedicinTime = "22:00",

                    GroceryDay = DateTime.Today.AddDays(1),
                    PaymentMethod = "Mobilepay"
                },
                new()
                {
                    ResidentCardId = Guid.NewGuid(),
                    ResidentId = Guid.NewGuid(),
                    Date = DateTime.Today,

                    ResidentName = "Mette Nielsen",
                    IsActive = true,

                    RiskLevel = "Rød",
                    Status = "Ked af det",


                    PnTime = "20:00",
                    MedicinTime = "22:00",

                    GroceryDay = DateTime.Today.AddDays(1),
                    PaymentMethod = "Mobilepay"
                },
                new()
                {
                    ResidentCardId = Guid.NewGuid(),
                    ResidentId = Guid.NewGuid(),
                    Date = DateTime.Today,

                    ResidentName = "Ole Pontoppidan",
                    IsActive = true,

                    RiskLevel = "Rød",
                    Status = "Ked af det",


                    PnTime = "20:00",
                    MedicinTime = "22:00",

                    GroceryDay = DateTime.Today.AddDays(1),
                    PaymentMethod = "Mobilepay"
                },
                new()
                {
                    ResidentCardId = Guid.NewGuid(),
                    ResidentId = Guid.NewGuid(),
                    Date = DateTime.Today,

                    ResidentName = "Quint Roberts",
                    IsActive = true,

                    RiskLevel = "Rød",
                    Status = "Ked af det",


                    PnTime = "20:00",
                    MedicinTime = "22:00",

                    GroceryDay = DateTime.Today.AddDays(1),
                    PaymentMethod = "Mobilepay"
                },
                new()
                {
                    ResidentCardId = Guid.NewGuid(),
                    ResidentId = Guid.NewGuid(),
                    Date = DateTime.Today,

                    ResidentName = "Søren Thomasson",
                    IsActive = true,

                    RiskLevel = "Rød",
                    Status = "Ked af det",


                    PnTime = "20:00",
                    MedicinTime = "22:00",

                    GroceryDay = DateTime.Today.AddDays(1),
                    PaymentMethod = "Mobilepay"
                },
                new()
                {
                    ResidentCardId = Guid.NewGuid(),
                    ResidentId = Guid.NewGuid(),
                    Date = DateTime.Today,

                    ResidentName = "Ulke Venja",
                    IsActive = true,

                    RiskLevel = "Rød",
                    Status = "Ked af det",


                    PnTime = "20:00",
                    MedicinTime = "22:00",

                    GroceryDay = DateTime.Today.AddDays(1),
                    PaymentMethod = "Mobilepay"
                },
                new()
                {
                    ResidentCardId = Guid.NewGuid(),
                    ResidentId = Guid.NewGuid(),
                    Date = DateTime.Today,

                    ResidentName = "Whilmer Xander",
                    IsActive = true,

                    RiskLevel = "Rød",
                    Status = "Ked af det",


                    PnTime = "20:00",
                    MedicinTime = "22:00",

                    GroceryDay = DateTime.Today.AddDays(1),
                    PaymentMethod = "Kontanter"
                },

                new()
                {
                    ResidentCardId = Guid.NewGuid(),
                    ResidentId = Guid.NewGuid(),
                    Date = DateTime.Today,

                    ResidentName = "Yvonne Zeniassen  Bjórnsdóttir",
                    IsActive = true,

                    RiskLevel = "Rød",
                    Status = "Ked af det",


                    PnTime = "20:00",
                    MedicinTime = "22:00",

                    GroceryDay = DateTime.Today.AddDays(1),
                    PaymentMethod = "Kontanter"
                }
            }
            };
        }
    }
}
