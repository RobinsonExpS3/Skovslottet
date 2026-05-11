using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Slottet.Client.Pages.AdminPages;
using Slottet.Shared;
using Slottet.Client.Test.Helpers;


namespace Slottet.Client.Test
{
    [TestClass]
    public class AdminResidentTests : BunitContext
    {
        private FakeHttpMessageHandler _handler = null!;

        [TestInitialize]
        public void Setup()
        {
            _handler = new FakeHttpMessageHandler();

            Services.AddSingleton(new HttpClient(_handler)
            {
                BaseAddress = new Uri("http://localhost:7201")
            });

        }

        //Helpers
        private void SetupSuccessfulInitialLoad()
        {
            _handler.AddStatus(HttpMethod.Get, "api/shiftboard/current", HttpStatusCode.OK);

            _handler.AddJson(HttpMethod.Get, "api/Resident/cards", new List<ResidentCardDto>
            {
                new()
                {
                    ResidentID       = Guid.NewGuid(),
                    ResidentStatusID = Guid.NewGuid(),
                    ResidentName     = "Anna Hansen",
                    IsActive         = true,
                    GroceryDay       = "Mandag",
                    AssignedStaff    = new List<string>(),
                    MedicineSchedule = new List<MedicineScheduleItemDto>(),
                    PNEntries        = new List<PNEntryDto>()
                }
            });

            _handler.AddJson(HttpMethod.Get, "api/Staff/Staffs", new List<EditStaffDTO>());
            _handler.AddJson(HttpMethod.Get, "api/Resident/groceryDays", new List<ResidentLookupDTO>
            {
                new() { ID = Guid.NewGuid(), Name = "Mandag" }
            });
            _handler.AddJson(HttpMethod.Get, "api/Resident/paymentMethods", new List<ResidentLookupDTO>());
        }

        private void SetupSuccessfulInitialLoadWithResidentId(Guid residentId)
        {
            _handler.AddStatus(HttpMethod.Get, "api/shiftboard/current", HttpStatusCode.OK);

            _handler.AddJson(HttpMethod.Get, "api/Resident/cards", new List<ResidentCardDto>
            {
                new()
                {
                    ResidentID       = residentId,
                    ResidentStatusID = Guid.NewGuid(),
                    ResidentName     = "Anna Hansen",
                    IsActive         = true,
                    GroceryDay       = "Mandag",
                    AssignedStaff    = new List<string>(),
                    MedicineSchedule = new List<MedicineScheduleItemDto>(),
                    PNEntries        = new List<PNEntryDto>()
                }
            });

            _handler.AddJson(HttpMethod.Get, "api/Staff/Staffs", new List<EditStaffDTO>());
            _handler.AddJson(HttpMethod.Get, "api/Resident/groceryDays", new List<ResidentLookupDTO>
            {
                new() { ID = Guid.NewGuid(), Name = "Mandag" }
            });
            _handler.AddJson(HttpMethod.Get, "api/Resident/paymentMethods", new List<ResidentLookupDTO>());
        }

        private static string Path(HttpRequestMessage request) =>
            request.RequestUri!.PathAndQuery.TrimStart('/');

        private bool WasCalled(HttpMethod method, string path) =>
            _handler.Requests.Any(r => r.Method == method && Path(r) == path);

        //Tests
        [TestMethod]
        public void AdminResident_LoadsResidentCards_WhenApiReturnsData()
        {
            SetupSuccessfulInitialLoad();

            var component = Render<AdminResident>();

            component.WaitForAssertion(() =>
            {
                Assert.Contains("Anna Hansen", component.Markup);
            });
        }

        [TestMethod]
        public void AdminResident_CallsExpectedEndpoints_WhenInitialized()
        {
            SetupSuccessfulInitialLoad();

            var component = Render<AdminResident>();

            component.WaitForAssertion(() =>
            {
                Assert.IsTrue(WasCalled(HttpMethod.Get, "api/shiftboard/current"));
                Assert.IsTrue(WasCalled(HttpMethod.Get, "api/Resident/cards"));
                Assert.IsTrue(WasCalled(HttpMethod.Get, "api/Staff/Staffs"));
                Assert.IsTrue(WasCalled(HttpMethod.Get, "api/Resident/groceryDays"));
                Assert.IsTrue(WasCalled(HttpMethod.Get, "api/Resident/paymentMethods"));
            });
        }

        [TestMethod]
        public void AdminResident_ShowsAccessDeniedMessage_WhenShiftBoardReturnsForbidden()
        {
            _handler.AddStatus(HttpMethod.Get, "api/shiftboard/current", HttpStatusCode.Forbidden);
            _handler.AddJson(HttpMethod.Get, "api/Resident/cards", new List<ResidentCardDto>());
            _handler.AddJson(HttpMethod.Get, "api/Staff/Staffs", new List<EditStaffDTO>());
            _handler.AddJson(HttpMethod.Get, "api/Resident/groceryDays", new List<ResidentLookupDTO>());
            _handler.AddJson(HttpMethod.Get, "api/Resident/paymentMethods", new List<ResidentLookupDTO>());

            var component = Render<AdminResident>();

            component.WaitForAssertion(() =>
            {
                Assert.Contains("Du har ikke adgang til denne side.", component.Markup);
            });
        }

        [TestMethod]
        public void AdminResident_ShowsLoadError_WhenShiftBoardReturnsOtherError()
        {
            _handler.AddStatus(HttpMethod.Get, "api/shiftboard/current", HttpStatusCode.InternalServerError);
            _handler.AddJson(HttpMethod.Get, "api/Resident/cards", new List<ResidentCardDto>());
            _handler.AddJson(HttpMethod.Get, "api/Staff/Staffs", new List<EditStaffDTO>());
            _handler.AddJson(HttpMethod.Get, "api/Resident/groceryDays", new List<ResidentLookupDTO>());
            _handler.AddJson(HttpMethod.Get, "api/Resident/paymentMethods", new List<ResidentLookupDTO>());

            var component = Render<AdminResident>();

            component.WaitForAssertion(() =>
            {
                Assert.Contains("Kunne ikke loade siden. Prøv venligst igen senere.", component.Markup);
            });
        }

        [TestMethod]
        public void AdminResident_ShowsLoadError_WhenResidentCardsLoadFails()
        {
            _handler.AddStatus(HttpMethod.Get, "api/shiftboard/current", HttpStatusCode.OK);
            _handler.AddStatus(HttpMethod.Get, "api/Resident/cards", HttpStatusCode.InternalServerError);
            _handler.AddJson(HttpMethod.Get, "api/Staff/Staffs", new List<EditStaffDTO>());
            _handler.AddJson(HttpMethod.Get, "api/Resident/groceryDays", new List<ResidentLookupDTO>());
            _handler.AddJson(HttpMethod.Get, "api/Resident/paymentMethods", new List<ResidentLookupDTO>());

            var component = Render<AdminResident>();

            component.WaitForAssertion(() =>
            {
                Assert.Contains("Fejl:", component.Markup);
            });
        }

        [TestMethod]
        public void AdminResident_CreateButtonIsDisabled_WhenNameIsMissing()
        {
            SetupSuccessfulInitialLoad();

            var component = Render<AdminResident>();

            component.WaitForAssertion(() =>
            {
                Assert.Contains("Anna Hansen", component.Markup);
            });

            // No name entered, no grocery day selected
            Assert.IsTrue(component.Find("button.new-shiftboard-button").HasAttribute("disabled"));
        }

        [TestMethod]
        public void AdminResident_CreateButtonIsDisabled_WhenGroceryDayIsNotSelected()
        {
            SetupSuccessfulInitialLoad();

            var component = Render<AdminResident>();

            component.WaitForAssertion(() =>
            {
                Assert.Contains("Anna Hansen", component.Markup);
            });

            // Name entered but grocery day still unselected
            component.Find("input[placeholder='Navn…']").Input("Bo Berg");

            Assert.IsTrue(component.Find("button.new-shiftboard-button").HasAttribute("disabled"));
        }

        [TestMethod]
        public void AdminResident_Create_PostsToResidentEndpoint_WhenFormIsValid()
        {
            var groceryDayId = Guid.NewGuid();

            _handler.AddStatus(HttpMethod.Get, "api/shiftboard/current", HttpStatusCode.OK);
            _handler.AddJson(HttpMethod.Get, "api/Resident/cards", new List<ResidentCardDto>
            {
                new()
                {
                    ResidentID       = Guid.NewGuid(),
                    ResidentStatusID = Guid.NewGuid(),
                    ResidentName     = "Anna Hansen",
                    IsActive         = true,
                    GroceryDay       = "Mandag",
                    AssignedStaff    = new List<string>(),
                    MedicineSchedule = new List<MedicineScheduleItemDto>(),
                    PNEntries        = new List<PNEntryDto>()
                }
            });
            _handler.AddJson(HttpMethod.Get, "api/Staff/Staffs", new List<EditStaffDTO>());
            _handler.AddJson(HttpMethod.Get, "api/Resident/groceryDays", new List<ResidentLookupDTO>
            {
                new() { ID = groceryDayId, Name = "Mandag" }
            });
            _handler.AddJson(HttpMethod.Get, "api/Resident/paymentMethods", new List<ResidentLookupDTO>());
            _handler.AddStatus(HttpMethod.Post, "api/Resident", HttpStatusCode.OK);

            var component = Render<AdminResident>();

            component.WaitForAssertion(() =>
            {
                Assert.Contains("Anna Hansen", component.Markup);
            });

            component.Find("input[placeholder='Navn…']").Input("Bo Berg");
            component.FindAll("select")[0].Change(groceryDayId.ToString()); // grocery day is the first select

            component.Find("button.new-shiftboard-button").Click();

            component.WaitForAssertion(() =>
            {
                Assert.IsTrue(WasCalled(HttpMethod.Post, "api/Resident"));
            });
        }
    }
}
