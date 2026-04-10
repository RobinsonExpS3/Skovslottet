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
                        RiskLevel = "Grøn",
                        LatestStatusNote = "Har sovet godt og taget morgenmedicin uden problemer.",
                        AssignedStaff = new List<string> { "Mette", "Jonas" },
                        MedicineSchedule = CreateMedicineSchedule(
                            ("kl 8", 8, true),
                            ("kl 9", 9, false),
                            ("kl 15", 15, false),
                            ("kl 22", 22, false)),
                        PNStatus = "",
                        PaymentMethod = "Dankort",
                        GroceryDay = "Mandag"
                    },
                    new()
                    {
                        ResidentCardId = Guid.NewGuid(),
                        ResidentId = Guid.NewGuid(),
                        Date = DateTime.Today,
                        ResidentName = "Carsten Didriksen",
                        IsActive = true,
                        RiskLevel = "Grøn",
                        LatestStatusNote = "Rolig formiddag. Har spist og været ude at gå.",
                        AssignedStaff = new List<string> { "Søren", "Emma" },
                        MedicineSchedule = CreateMedicineSchedule(
                            ("kl 9", 9, true),
                            ("kl 10", 10, false),
                            ("kl 11", 11, false),
                            ("kl 15", 15, false),
                            ("kl 21", 21, false),
                            ("kl 22", 22, false)),
                        PNStatus = "",
                        PaymentMethod = "Mobilepay og Dankort",
                        GroceryDay = "Mandag"
                    },
                    new()
                    {
                        ResidentCardId = Guid.NewGuid(),
                        ResidentId = Guid.NewGuid(),
                        Date = DateTime.Today,
                        ResidentName = "Enaya Frederiksen",
                        IsActive = true,
                        RiskLevel = "Gul",
                        LatestStatusNote = "Virker nedtrykt og ønsker ro i dag.",
                        AssignedStaff = new List<string> { "Lene", "Michael" },
                        MedicineSchedule = CreateMedicineSchedule(
                            ("kl 8", 8, false),
                            ("kl 9", 9, false),
                            ("kl 12", 12, true),
                            ("kl 15", 15, false),
                            ("kl 22", 22, true)),
                        PNStatus = "",
                        PaymentMethod = "Kontanter",
                        GroceryDay = "Mandag"
                    },
                    new()
                    {
                        ResidentCardId = Guid.NewGuid(),
                        ResidentId = Guid.NewGuid(),
                        Date = DateTime.Today,
                        ResidentName = "Gert Heller",
                        IsActive = true,
                        RiskLevel = "Gul",
                        LatestStatusNote = "Har brug for ekstra støtte og tydelig guidning.",
                        AssignedStaff = new List<string> { "Nadia", "Ole" },
                        MedicineSchedule = CreateMedicineSchedule(
                            ("kl 8", 8, true),
                            ("kl 22", 22, false)),
                        PNStatus = "",
                        PaymentMethod = "Mobilepay og Dankort",
                        GroceryDay = "Mandag"
                    },
                    new()
                    {
                        ResidentCardId = Guid.NewGuid(),
                        ResidentId = Guid.NewGuid(),
                        Date = DateTime.Today,
                        ResidentName = "Ida Jacoby",
                        IsActive = true,
                        RiskLevel = "Rød",
                        LatestStatusNote = "Har været urolig i løbet af natten.",
                        AssignedStaff = new List<string> { "Freja", "Andreas" },
                        MedicineSchedule = CreateMedicineSchedule(),
                        PNStatus = "",
                        PaymentMethod = "Mobilepay og Dankort",
                        GroceryDay = "Mandag"
                    },
                    new()
                    {
                        ResidentCardId = Guid.NewGuid(),
                        ResidentId = Guid.NewGuid(),
                        Date = DateTime.Today,
                        ResidentName = "Karl Larsen",
                        IsActive = true,
                        RiskLevel = "Rød",
                        LatestStatusNote = "Vil gerne have faste rutiner og korte beskeder.",
                        AssignedStaff = new List<string> { "Kasper", "Liva" },
                        MedicineSchedule = CreateMedicineSchedule(
                            ("kl 9", 9, false),
                            ("kl 10", 10, true),
                            ("kl 11", 11, false)),
                        PNStatus = "",
                        PaymentMethod = "Mobilepay og Dankort",
                        GroceryDay = "Mandag"
                    },
                    new()
                    {
                        ResidentCardId = Guid.NewGuid(),
                        ResidentId = Guid.NewGuid(),
                        Date = DateTime.Today,
                        ResidentName = "Mette Nielsen",
                        IsActive = true,
                        RiskLevel = "Gul",
                        LatestStatusNote = "Følsom over for støj i fællesarealerne.",
                        AssignedStaff = new List<string> { "Mads", "Signe" },
                        MedicineSchedule = CreateMedicineSchedule(
                            ("kl 8", 8, true),
                            ("kl 12", 12, false),
                            ("kl 21", 21, false),
                            ("kl 22", 22, false)),
                        PNStatus = "",
                        PaymentMethod = "Mobilepay og Dankort",
                        GroceryDay = "Mandag"
                    },
                    new()
                    {
                        ResidentCardId = Guid.NewGuid(),
                        ResidentId = Guid.NewGuid(),
                        Date = DateTime.Today,
                        ResidentName = "Ole Pontoppidan",
                        IsActive = true,
                        RiskLevel = "Gul",
                        LatestStatusNote = "Har brug for opmuntring før aktiviteter.",
                        AssignedStaff = new List<string> { "Jonas", "Mia" },
                        MedicineSchedule = CreateMedicineSchedule(
                            ("kl 8", 8, false),
                            ("kl 22", 22, true)),
                        PNStatus = "",
                        PaymentMethod = "Mobilepay og Dankort",
                        GroceryDay = "Mandag"
                    },
                    new()
                    {
                        ResidentCardId = Guid.NewGuid(),
                        ResidentId = Guid.NewGuid(),
                        Date = DateTime.Today,
                        ResidentName = "Quint Roberts",
                        IsActive = true,
                        RiskLevel = "Gul",
                        LatestStatusNote = "Har været afventende i kontakt med personale.",
                        AssignedStaff = new List<string> { "Noah", "Lærke" },
                        MedicineSchedule = CreateMedicineSchedule(
                            ("kl 8", 8, true),
                            ("kl 12", 12, false),
                            ("kl 22", 22, false)),
                        PNStatus = "",
                        PaymentMethod = "Mobilepay og Dankort",
                        GroceryDay = "Mandag"
                    },
                    new()
                    {
                        ResidentCardId = Guid.NewGuid(),
                        ResidentId = Guid.NewGuid(),
                        Date = DateTime.Today,
                        ResidentName = "Søren Thomasson",
                        IsActive = true,
                        RiskLevel = "Rød",
                        LatestStatusNote = "Har haft behov for hyppige pauser.",
                        AssignedStaff = new List<string> { "Peter", "Camilla" },
                        MedicineSchedule = CreateMedicineSchedule(
                            ("kl 8", 8, false),
                            ("kl 22", 22, false)),
                        PNStatus = "",
                        PaymentMethod = "Mobilepay og Dankort",
                        GroceryDay = "Mandag"
                    },
                    new()
                    {
                        ResidentCardId = Guid.NewGuid(),
                        ResidentId = Guid.NewGuid(),
                        Date = DateTime.Today,
                        ResidentName = "Ulke Venja",
                        IsActive = true,
                        RiskLevel = "Rød",
                        LatestStatusNote = "Har været træt og ønsket at blive på værelset.",
                        AssignedStaff = new List<string> { "Rikke", "Thomas" },
                        MedicineSchedule = CreateMedicineSchedule(
                            ("kl 8", 8, true),
                            ("kl 9", 9, true),
                            ("kl 10", 10, false),
                            ("kl 11", 11, false),
                            ("kl 12", 12, false),
                            ("kl 15", 15, true),
                            ("kl 20", 20, false),
                            ("kl 21", 21, false),
                            ("kl 22", 22, false)),
                        PNStatus = "",
                        PaymentMethod = "Mobilepay og Dankort",
                        GroceryDay = "Mandag"
                    },
                    new()
                    {
                        ResidentCardId = Guid.NewGuid(),
                        ResidentId = Guid.NewGuid(),
                        Date = DateTime.Today,
                        ResidentName = "Whilmer Xander",
                        IsActive = true,
                        RiskLevel = "Grøn",
                        LatestStatusNote = "Ønsker rolig kontakt og én medarbejder ad gangen.",
                        AssignedStaff = new List<string> { "Louise", "Emil" },
                        MedicineSchedule = CreateMedicineSchedule(
                            ("kl 8", 8, false),
                            ("kl 9", 9, true),
                            ("kl 10", 10, false),
                            ("kl 22", 22, false)),
                        PNStatus = "Panodil kl. 7",
                        PaymentMethod = "Mobilepay og Dankort",
                        GroceryDay = "Mandag"
                    },
                    new()
                    {
                        ResidentCardId = Guid.NewGuid(),
                        ResidentId = Guid.NewGuid(),
                        Date = DateTime.Today,
                        ResidentName = "Yvonne Zeniassen Bjórnsdóttir",
                        IsActive = true,
                        RiskLevel = "Grøn",
                        LatestStatusNote = "Har haft en svær start på dagen, men samarbejder fint nu.",
                        AssignedStaff = new List<string> { "Helene", "Victor" },
                        MedicineSchedule = CreateMedicineSchedule(
                            ("kl 8", 8, true),
                            ("kl 12", 12, false),
                            ("kl 15", 15, false),
                            ("kl 22", 22, true)),
                        PNStatus = "Panodil kl. 20",
                        PaymentMethod = "Mobilepay og Dankort",
                        GroceryDay = "Mandag"
                    }
                }
            };
        }

        private static List<MedicineScheduleItemDto> CreateMedicineSchedule(params (string label, int hour, bool isGiven)[] items)
            {
                return items
                    .Select(x => new MedicineScheduleItemDto
                    {
                        Label = x.label,
                        Time = new TimeOnly(x.hour, 0),
                        IsGiven = x.isGiven
                    })
                    .ToList();
            }

        //private static List<PnScheduleItemDto> CreatePnSchedule(int count, params int[] givenIndexes)
        //    {
        //        var givenSet = givenIndexes.ToHashSet();

        //        return Enumerable.Range(0, count)
        //            .Select(i => new PnScheduleItemDto
        //            {
        //                Label = "PN",
        //                TimeLabel = string.Empty,
        //                IsGiven = givenSet.Contains(i)
        //            })
        //            .ToList();
        //    }
    };
}
  
