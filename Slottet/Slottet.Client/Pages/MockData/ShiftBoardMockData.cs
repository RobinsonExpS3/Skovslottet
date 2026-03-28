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
                DepartmentName = "Afdeling Solsikken",

                PhoneNumbers = new List<string>
            {
                "12 34 56 78",
                "87 65 43 21"
            },

                DepartmentTasks = new List<string>
            {
                "Morgenmedicin",
                "Dokumentation",
                "Kontakt til pårørende"
            },

                SpecialResponsibilities = new List<string>
            {
                "Brandansvarlig",
                "Nøgleansvarlig"
            },

                ResidentCards = new List<ResidentCardViewDto>
            {
                new()
                {
                    ResidentCardId = Guid.NewGuid(),
                    ResidentId = Guid.NewGuid(),
                    ResidentName = "Anna Jensen",
                    IsActive = true,
                    GroceryDay = DateTime.Today.AddDays(1),
                    Status = "Stabil",
                    Date = DateTime.Now,
                    RiskLevel = "Lav",
                    PnTime = "20:00"
                },
                new()
                {
                    ResidentCardId = Guid.NewGuid(),
                    ResidentId = Guid.NewGuid(),
                    ResidentName = "Bent Larsen",
                    IsActive = true,
                    GroceryDay = DateTime.Today.AddDays(2),
                    Status = "Obs",
                    Date = DateTime.Now,
                    RiskLevel = "Mellem",
                    PnTime = "21:00"
                },
                new()
                {
                    ResidentCardId = Guid.NewGuid(),
                    ResidentId = Guid.NewGuid(),
                    ResidentName = "Clara Nielsen",
                    IsActive = false,
                    GroceryDay = null,
                    Status = "Passiv",
                    Date = DateTime.Now,
                    RiskLevel = "Høj",
                    PnTime = "Efter behov"
                }
            }
            };
        }
    }
}
