using System.Net;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Slottet.Client.Pages.ShiftBoard;
using Slottet.Client.Test.Helpers;
using Slottet.Shared;

namespace Slottet.Client.Test {
    /// <summary>
    /// bUnit tests for the ShiftBoard page.
    /// Verifies rendering, API interaction, authorization handling,
    /// navigation, and overlay functionality.
    /// </summary>
    [TestClass]
    public class ShiftBoardTests : BunitContext {
        // Fake HTTP handler used to mock backend API responses.
        private FakeHttpMessageHandler _handler = null!;

        /// <summary>
        /// Configures the fake HTTP client and dependency injection
        /// before each test executes.
        /// </summary>
        [TestInitialize]
        public void Setup() {
            _handler = new FakeHttpMessageHandler();

            Services.AddSingleton(new HttpClient(_handler) {
                BaseAddress = new Uri("https://localhost:7201/")
            });
        }

        // Helpers

        /// <summary>
        /// Extracts the relative request path from an HTTP request message.
        /// </summary>
        private static string Path(HttpRequestMessage request) =>
            request.RequestUri!.PathAndQuery.TrimStart('/');

        /// <summary>
        /// Checks whether a request with the specified HTTP method and path
        /// was executed during the test.
        /// </summary>
        private bool WasCalled(HttpMethod method, string path) =>
            _handler.Requests.Any(r => r.Method == method && Path(r) == path);

        /// <summary>
        /// Calculates the current shift type based on the provided time.
        /// Defaults to the current system time when no value is supplied.
        /// </summary>
        private static string CurrentShiftType(DateTime? at = null) {
            var hour = (at ?? DateTime.Now).Hour;

            return hour switch {
                >= 7 and < 15 => "Dag",
                >= 15 and < 23 => "Aften",
                _ => "Nat"
            };
        }

        /// <summary>
        /// Calculates the active shift date.
        /// Night shifts before 07:00 belong to the previous calendar day.
        /// </summary>
        private static DateOnly CurrentShiftDate(DateTime? at = null) {
            var now = at ?? DateTime.Now;

            return now.Hour < 7
                ? DateOnly.FromDateTime(now.AddDays(-1))
                : DateOnly.FromDateTime(now);
        }

        /// <summary>
        /// Builds the expected API URL for retrieving the current shift board.
        /// </summary>
        private static string CurrentShiftUrl() {
            var date = CurrentShiftDate();
            var shift = CurrentShiftType();

            return $"api/shiftboard/by-shift?date={date:yyyy-MM-dd}&shiftType={shift}";
        }

        /// <summary>
        /// Configures successful mock responses for the current shift board page load.
        /// </summary>
        private void SetupSuccessfulInitialLoad(ShiftBoardDTO? dto = null) {
            _handler.AddStatus(HttpMethod.Get, "api/shiftboard/current", HttpStatusCode.OK);
            _handler.AddJson(HttpMethod.Get, CurrentShiftUrl(), dto ?? ValidShiftBoardDto());
        }

        /// <summary>
        /// Creates a reusable valid ShiftBoardDTO instance for test scenarios.
        /// </summary>
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

        /// <summary>
        /// Ensures the shift board page renders data returned from the API.
        /// Verifies that the bUnit-rendered markup contains department, resident, phone, responsibility, and task values from the mocked ShiftBoardDTO.
        /// </summary>
        [TestMethod]
        public void ShiftBoard_LoadsShiftBoard_WhenApiReturnsData() {
            SetupSuccessfulInitialLoad();

            var component = Render<ShiftBoard>();

            component.WaitForAssertion(() => {
                Assert.Contains("Slottet", component.Markup);
                Assert.Contains("John Doe", component.Markup);
                Assert.Contains("Har haft en rolig morgen", component.Markup);
                Assert.Contains("12345678", component.Markup);
                Assert.Contains("Medicinansvar", component.Markup);
                Assert.Contains("Tøm opvaskemaskine", component.Markup);
            });
        }

        /// <summary>
        /// Ensures ShiftBoard requests the current access check and active shift data during initialization.
        /// Verifies that the bUnit render triggers GET calls to the current shiftboard endpoint and the calculated shift URL.
        /// </summary>
        [TestMethod]
        public void ShiftBoard_CallsExpectedEndpoints_WhenInitialized() {
            SetupSuccessfulInitialLoad();

            var component = Render<ShiftBoard>();

            component.WaitForAssertion(() => {
                Assert.IsTrue(WasCalled(HttpMethod.Get, "api/shiftboard/current"));
                Assert.IsTrue(WasCalled(HttpMethod.Get, CurrentShiftUrl()));
            });
        }

        /// <summary>
        /// Ensures forbidden API responses are handled as an access denied state.
        /// Verifies that the rendered component shows the access denied message when shiftboard endpoints return 403 Forbidden.
        /// </summary>
        [TestMethod]
        public void ShiftBoard_ShowsAccessDenied_WhenCurrentAndShiftReturnForbidden() {
            _handler.AddStatus(HttpMethod.Get, "api/shiftboard/current", HttpStatusCode.Forbidden);
            _handler.AddStatus(HttpMethod.Get, CurrentShiftUrl(), HttpStatusCode.Forbidden);

            var component = Render<ShiftBoard>();

            component.WaitForAssertion(() => {
                Assert.Contains("Du har ikke adgang til denne side.", component.Markup);
            });
        }

        /// <summary>
        /// Ensures a failed shift board load is surfaced as a general load error.
        /// Verifies that an internal server error from the active shift endpoint renders the expected error message.
        /// </summary>
        [TestMethod]
        public void ShiftBoard_ShowsLoadError_WhenShiftLoadFails() {
            _handler.AddStatus(HttpMethod.Get, "api/shiftboard/current", HttpStatusCode.OK);
            _handler.AddStatus(HttpMethod.Get, CurrentShiftUrl(), HttpStatusCode.InternalServerError);

            var component = Render<ShiftBoard>();

            component.WaitForAssertion(() => {
                Assert.Contains("Kunne ikke loade data", component.Markup);
            });
        }

        /// <summary>
        /// Ensures the page handles a missing shift board without rendering shift data.
        /// Verifies that a null ShiftBoardDTO response renders the empty shift board message.
        /// </summary>
        [TestMethod]
        public void ShiftBoard_ShowsEmptyMessage_WhenNoShiftBoardExists() {
            _handler.AddStatus(HttpMethod.Get, "api/shiftboard/current", HttpStatusCode.OK);
            _handler.AddJson<ShiftBoardDTO?>(HttpMethod.Get, CurrentShiftUrl(), null);

            var component = Render<ShiftBoard>();

            component.WaitForAssertion(() => {
                Assert.Contains("Ingen vagttavle oprettet", component.Markup);
            });
        }

        /// <summary>
        /// Ensures the new shift board action navigates to the admin staff page.
        /// Verifies that clicking the new shift board button updates NavigationManager.Uri to /adminStaff.
        /// </summary>
        [TestMethod]
        public void ShiftBoard_NavigateToNewShiftBoard_GoesToAdminStaff() {
            SetupSuccessfulInitialLoad();

            var component = Render<ShiftBoard>();

            component.WaitForAssertion(() => {
                Assert.Contains("Ny vagt-tavle", component.Markup);
            });

            // Trigger navigation to the admin staff page.
            component.Find("button.new-shiftboard-button").Click();

            var nav = Services.GetRequiredService<NavigationManager>();

            // Verify navigation target.
            Assert.EndsWith("/adminStaff", nav.Uri);
        }

        /// <summary>
        /// Ensures the phone list info box opens its overlay panel.
        /// Verifies that a bUnit click event on the phone list box renders the phone list overlay after JS interop is mocked.
        /// </summary>
        [TestMethod]
        public void ShiftBoard_OpenPhoneList_ShowsPhoneListOverlay() {
            SetupSuccessfulInitialLoad();

            // Mock overlay animation helper.
            JSInterop.SetupVoid("overlayHelpers.playFlyIn", _ => true);

            var component = Render<ShiftBoard>();

            component.WaitForAssertion(() => {
                Assert.Contains("Telefonnumre", component.Markup);
            });

            // Open phone list overlay.
            component.FindAll(".info-box--clickable")[0].Click();

            component.WaitForAssertion(() => {
                Assert.Contains("overlay-panel-phonelist", component.Markup);
            });
        }

        /// <summary>
        /// Ensures the department tasks info box opens its overlay panel.
        /// Verifies that a bUnit click event on the department tasks box renders the department tasks overlay after JS interop is mocked.
        /// </summary>
        [TestMethod]
        public void ShiftBoard_OpenDepartmentTasks_ShowsDepartmentTaskOverlay() {
            SetupSuccessfulInitialLoad();

            // Mock overlay animation helper.
            JSInterop.SetupVoid("overlayHelpers.playFlyIn", _ => true);

            var component = Render<ShiftBoard>();

            component.WaitForAssertion(() => {
                Assert.Contains("Afdelingens opgaver", component.Markup);
            });

            // Open department tasks overlay.
            component.FindAll(".info-box--clickable")[2].Click();

            component.WaitForAssertion(() => {
                Assert.Contains("overlay-panel-departmenttasks", component.Markup);
            });
        }

        /// <summary>
        /// Ensures the special responsibilities info box opens its overlay panel.
        /// Verifies that a bUnit click event on the special responsibilities box renders the special responsibilities overlay after JS interop is mocked.
        /// </summary>
        [TestMethod]
        public void ShiftBoard_OpenSpecialResponsibilities_ShowsSpecialResponsibilityOverlay() {
            SetupSuccessfulInitialLoad();

            // Mock overlay animation helper.
            JSInterop.SetupVoid("overlayHelpers.playFlyIn", _ => true);

            var component = Render<ShiftBoard>();

            component.WaitForAssertion(() => {
                Assert.Contains("Særligt ansvar", component.Markup);
            });

            // Open special responsibilities overlay.
            component.FindAll(".info-box--clickable")[1].Click();

            component.WaitForAssertion(() => {
                Assert.Contains("overlay-panel-specialresponsibilities", component.Markup);
            });
        }
    }
}
