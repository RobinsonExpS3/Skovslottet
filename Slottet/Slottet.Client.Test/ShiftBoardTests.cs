using System.Net;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Slottet.Client.Pages.ShiftBoard;
using Slottet.Client.Test.Helpers;
using Slottet.Shared;

namespace Slottet.Client.Test {
    [TestClass]
    public class ShiftBoardTests : BunitContext {
        private FakeHttpMessageHandler _handler = null!;


        [TestInitialize]
        public void Setup() {
            _handler = new FakeHttpMessageHandler();

            Services.AddSingleton(new HttpClient(_handler) {
                BaseAddress = new Uri("https://localhost:7201/")
            });
        }

        // Helpers
        private static string Path(HttpRequestMessage request) =>
            request.RequestUri!.PathAndQuery.TrimStart('/');

        private bool WasCalled(HttpMethod method, string path) =>
            _handler.Requests.Any(r => r.Method == method && Path(r) == path);

        private static string CurrentShiftType(DateTime? at = null) {
            var hour = (at ?? DateTime.Now).Hour;

            return hour switch {
                >= 7 and < 15 => "Dag",
                >= 15 and < 23 => "Aften",
                _ => "Nat"
            };
        }

        private static DateOnly CurrentShiftDate(DateTime? at = null) {
            var now = at ?? DateTime.Now;

            return now.Hour < 7
                ? DateOnly.FromDateTime(now.AddDays(-1))
                : DateOnly.FromDateTime(now);
        }

        private static string CurrentShiftUrl() {
            var date = CurrentShiftDate();
            var shift = CurrentShiftType();

            return $"api/shiftboard/by-shift?date={date:yyyy-MM-dd}&shiftType={shift}";
        }

        private void SetupSuccessfulInitialLoad(ShiftBoardDTO? dto = null) {
            _handler.AddStatus(HttpMethod.Get, "api/shiftboard/current", HttpStatusCode.OK);
            _handler.AddJson(HttpMethod.Get, CurrentShiftUrl(), dto ?? ValidShiftBoardDto());
        }

        private static ShiftBoardDTO ValidShiftBoardDto() {
            return new ShiftBoardDTO {
                ShiftBoardId = Guid.NewGuid(),
                ShiftType = CurrentShiftType(),
                DepartmentName = "Slottet",
                StartDate = DateTime.Now.AddHours(-1),
                EndDate = DateTime.Now.AddHours(6),
                AllStaff = new List<string> { "Anna Hansen", "Peter Jensen" },
                ResidentCards = new List<ResidentCardDto> {
                    new() {
                        ResidentStatusID = Guid.NewGuid(),
                        ResidentID = Guid.NewGuid(),
                        ResidentName = "John Doe",
                        IsActive = true,
                        RiskLevel = "Grøn",
                        LatestStatusNote = "Har haft en rolig morgen",
                        PaymentMethod = "Kort",
                        GroceryDay = "Mandag",
                        AssignedStaff = new List<string> { "Anna Hansen" },
                        MedicineSchedule = new List<MedicineScheduleItemDto> {
                            new() { Label = "08:00", Time = new TimeOnly(8, 0), IsGiven = true }
                        },
                        PNEntries = new List<PNEntryDto> {
                            new() {
                                TimeOfAdministration = "09:00",
                                Medication = "Panodil",
                                Reason = "Hovedpine",
                                IssuedBy = "AH"
                            }
                        }
                    }
                },
                PhoneNumbers = new List<PhoneEntryDto> {
                    new() { Number = "12345678", StaffName = "Anna Hansen" }
                },
                SpecialResponsibilities = new List<SpecialResponsibilityEntryDto> {
                    new() { Description = "Medicinansvar", StaffName = "Peter Jensen" }
                },
                DepartmentTasks = new List<string> { "Tøm opvaskemaskine" }
            };
        }


    }
}
