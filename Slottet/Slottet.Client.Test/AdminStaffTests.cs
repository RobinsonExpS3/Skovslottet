using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Slottet.Client.Pages.AdminPages;
using Slottet.Shared;
using Slottet.Client.Test.Helpers;

namespace Slottet.Client.Test {
    [TestClass]
    public class AdminStaffTests : BunitContext {
        private FakeHttpMessageHandler _handler = null!;

        [TestInitialize]
        public void Setup() {
            _handler = new FakeHttpMessageHandler();

            Services.AddSingleton(new HttpClient(_handler) {
                BaseAddress = new Uri("https://localhost:7201/")
            });
        }

        // Helpers
        private void SetupSuccessfulInitialLoad() {
            _handler.AddStatus(HttpMethod.Get, "api/shiftboard/current", HttpStatusCode.OK);

            _handler.AddJson(HttpMethod.Get, "api/Department", new List<DepartmentLookupDto> {
                new() { ID = Guid.NewGuid(), Name = "Slottet" }
            });

            _handler.AddJson(HttpMethod.Get, "api/Staff/Staffs", new List<EditStaffDto> {
                new() {
                    StaffID = Guid.NewGuid(),
                    StaffName = "Anna Hansen",
                    Initials = "AH",
                    Role = "Pædagog",
                    DepartmentID = Guid.NewGuid()
                }
            });

            _handler.AddJson(HttpMethod.Get, "api/SpecialResponsibility/SpecialResponsibilities", 
                new List<SpecialResponsibilityEntryDto>());
        }

        private void SetupSuccessfulInitialLoadWithStaffId(Guid staffId) {
            _handler.AddStatus(HttpMethod.Get, "api/shiftboard/current", HttpStatusCode.OK);

            _handler.AddJson(HttpMethod.Get, "api/Department", new List<DepartmentLookupDto> { 
                new() { ID = Guid.NewGuid(), Name = "Slottet" } 
            });

            _handler.AddJson(HttpMethod.Get, "api/Staff/Staffs", new List<EditStaffDto> {
                new() {
                    StaffID = staffId,
                    StaffName = "Anna Hansen",
                    Initials = "AH",
                    Role = "staff",
                    DepartmentID = Guid.NewGuid()
                }
            });

            _handler.AddJson(
                HttpMethod.Get,
                "api/SpecialResponsibility/SpecialResponsibilities",
                new List<SpecialResponsibilityEntryDto>());
        }

        private static string Path(HttpRequestMessage request) =>
            request.RequestUri!.PathAndQuery.TrimStart('/');

        private bool WasCalled(HttpMethod method, string path) =>
            _handler.Requests.Any(r => r.Method == method && Path(r) == path);


        // Tests
        [TestMethod]
        public void AdminStaff_LoadsStaffMembers_WhenApiReturnsData() {
            SetupSuccessfulInitialLoad();

            var component = Render<AdminStaff>();

            component.WaitForAssertion(() =>
            {
                Assert.Contains("Anna Hansen", component.Markup);
                Assert.Contains("AH", component.Markup);
            });
        }

        [TestMethod]
        public void AdminStaff_CallsExpectedEndpoints_WhenInitialized() {
            SetupSuccessfulInitialLoad();

            var component = Render<AdminStaff>();

            component.WaitForAssertion(() => {
                Assert.IsTrue(WasCalled(HttpMethod.Get, "api/shiftboard/current"));
                Assert.IsTrue(WasCalled(HttpMethod.Get, "api/Department"));
                Assert.IsTrue(WasCalled(HttpMethod.Get, "api/Staff/Staffs"));
                Assert.IsTrue(WasCalled(HttpMethod.Get, "api/SpecialResponsibility/SpecialResponsibilities"));
            });
        }

        [TestMethod]
        public void AdminStaff_ShowsAccessDeniedMessage_WhenShiftBoardReturnsForbidden() {
            _handler.AddStatus(HttpMethod.Get, "api/shiftboard/current", HttpStatusCode.Forbidden);
            _handler.AddJson(HttpMethod.Get, "api/Department", new List<DepartmentLookupDto>());
            _handler.AddJson(HttpMethod.Get, "api/Staff/Staffs", new List<EditStaffDto>());
            _handler.AddJson(HttpMethod.Get, "api/SpecialResponsibility/SpecialResponsibilities",
                new List<SpecialResponsibilityEntryDto>());

            var component = Render<AdminStaff>();

            component.WaitForAssertion(() => {
                Assert.Contains("Du har ikke adgang til denne side.", component.Markup);
            });
        }

        [TestMethod]
        public void AdminStaff_ShowsAccessDeniedMessage_WhenShiftBoardReturnsUnauthorized() {
            _handler.AddStatus(HttpMethod.Get, "api/shiftboard/current", HttpStatusCode.Unauthorized);
            _handler.AddJson(HttpMethod.Get, "api/Department", new List<DepartmentLookupDto>());
            _handler.AddJson(HttpMethod.Get, "api/Staff/Staffs", new List<EditStaffDto>());
            _handler.AddJson(HttpMethod.Get, "api/SpecialResponsibility/SpecialResponsibilities",
                new List<SpecialResponsibilityEntryDto>());

            var component = Render<AdminStaff>();

            component.WaitForAssertion(() =>
            {
                Assert.Contains("Du har ikke adgang til denne side.", component.Markup);
            });
        }

        [TestMethod]
        public void AdminStaff_ShowsLoadError_WhenDepartmentLoadFails() {
            _handler.AddStatus(HttpMethod.Get, "api/shiftboard/current", HttpStatusCode.OK);
            _handler.AddStatus(HttpMethod.Get, "api/Department", HttpStatusCode.InternalServerError);

            _handler.AddJson(HttpMethod.Get, "api/Staff/Staffs", new List<EditStaffDto>());
            _handler.AddJson(HttpMethod.Get, "api/SpecialResponsibility/SpecialResponsibilities",
                new List<SpecialResponsibilityEntryDto>());

            var component = Render<AdminStaff>();

            component.WaitForAssertion(() =>
            {
                Assert.Contains("Kunne ikke loade data", component.Markup);
            });
        }

        [TestMethod]
        public void AdminStaff_ShowsStaffLoadError_WhenStaffLoadFails() {
            _handler.AddStatus(HttpMethod.Get, "api/shiftboard/current", HttpStatusCode.OK);

            _handler.AddJson(HttpMethod.Get, "api/Department", new List<DepartmentLookupDto> { 
                new() { ID = Guid.NewGuid(), Name = "Slottet" } 
            });

            _handler.AddStatus(HttpMethod.Get, "api/Staff/Staffs", HttpStatusCode.InternalServerError);

            _handler.AddJson(HttpMethod.Get, "api/SpecialResponsibility/SpecialResponsibilities",
                new List<SpecialResponsibilityEntryDto>());

            var component = Render<AdminStaff>();

            component.WaitForAssertion(() =>
            {
                Assert.Contains("Kunne ikke loade data", component.Markup);
            });
        }

        [TestMethod]
        public void AdminStaff_Create_PostsStaffAndUser_WhenFormIsValid() {
            SetupSuccessfulInitialLoad();

            _handler.AddStatus(HttpMethod.Post, "api/Staff", HttpStatusCode.OK);
            _handler.AddStatus(HttpMethod.Post, "api/Auth/createUserForStaff", HttpStatusCode.OK);

            var component = Render<AdminStaff>();

            component.WaitForAssertion(() =>
            {
                Assert.Contains("Anna Hansen", component.Markup);
            });

            component.Find("input[placeholder='Navn']").Input("Peter Jensen");
            component.Find("input[placeholder='Initialer']").Input("PJ");
            component.Find("select").Change("staff");

            component.Find("button.btn-create").Click();

            component.WaitForAssertion(() =>
            {
                Assert.IsTrue(WasCalled(HttpMethod.Post, "api/Staff"));
                Assert.IsTrue(WasCalled(HttpMethod.Post, "api/Auth/createUserForStaff"));
            });
        }

        [TestMethod]
        public void AdminStaff_ShowsError_WhenCreateStaffFails() {
            SetupSuccessfulInitialLoad();

            _handler.AddStatus(HttpMethod.Post, "api/Staff", HttpStatusCode.InternalServerError);

            var component = Render<AdminStaff>();

            component.WaitForAssertion(() =>
            {
                Assert.Contains("Anna Hansen", component.Markup);
            });

            component.Find("input[placeholder='Navn']").Input("Peter Jensen");
            component.Find("input[placeholder='Initialer']").Input("PJ");
            component.Find("select").Change("staff");

            component.Find("button.btn-create").Click();

            component.WaitForAssertion(() =>
            {
                Assert.Contains("Kunne ikke oprette medarbejder", component.Markup);
                Assert.IsFalse(WasCalled(HttpMethod.Post, "api/Auth/createUserForStaff"));
            });
        }

        [TestMethod]
        public void AdminStaff_ShowsPartialCreateError_WhenUserCreationFails() {
            SetupSuccessfulInitialLoad();

            _handler.AddStatus(HttpMethod.Post, "api/Staff", HttpStatusCode.OK);
            _handler.AddStatus(HttpMethod.Post, "api/Auth/createUserForStaff", HttpStatusCode.InternalServerError);

            var component = Render<AdminStaff>();

            component.WaitForAssertion(() =>
            {
                Assert.Contains("Anna Hansen", component.Markup);
            });

            component.Find("input[placeholder='Navn']").Input("Peter Jensen");
            component.Find("input[placeholder='Initialer']").Input("PJ");
            component.Find("select").Change("staff");

            component.Find("button.btn-create").Click();

            component.WaitForAssertion(() =>
            {
                Assert.Contains("Medarbejderen blev oprettet, men login kunne ikke oprettes.", component.Markup);
            });
        }

        [TestMethod]
        public void AdminStaff_SelectStaff_PopulatesFormAndEnablesUpdateAndDelete() {
            SetupSuccessfulInitialLoad();

            var component = Render<AdminStaff>();

            component.WaitForAssertion(() =>
            {
                Assert.Contains("Anna Hansen", component.Markup);
            });

            component.Find("tr.staff-row").Click();

            var nameInput = component.Find("input[placeholder='Navn']");
            var initialsInput = component.Find("input[placeholder='Initialer']");

            Assert.AreEqual("Anna Hansen", nameInput.GetAttribute("value"));
            Assert.AreEqual("AH", initialsInput.GetAttribute("value"));

            Assert.IsFalse(component.Find("button.btn-update").HasAttribute("disabled"));
            Assert.IsFalse(component.Find("button.btn-delete").HasAttribute("disabled"));
        }

        [TestMethod]
        public void AdminStaff_Delete_DeletesSelectedStaff_WhenStaffIsSelected() {
            var staffId = Guid.NewGuid();

            SetupSuccessfulInitialLoadWithStaffId(staffId);

            _handler.AddStatus(HttpMethod.Delete, $"api/Staff/{staffId}", HttpStatusCode.OK);

            var component = Render<AdminStaff>();

            component.WaitForAssertion(() =>
            {
                Assert.Contains("Anna Hansen", component.Markup);
            });

            component.Find("tr.staff-row").Click();
            component.Find("button.btn-delete").Click();

            component.WaitForAssertion(() =>
            {
                Assert.IsTrue(WasCalled(HttpMethod.Delete, $"api/Staff/{staffId}"));
            });
        }

        [TestMethod]
        public void AdminStaff_UpdateAndDeleteButtonsAreDisabled_WhenNoStaffIsSelected() {
            SetupSuccessfulInitialLoad();

            var component = Render<AdminStaff>();

            component.WaitForAssertion(() =>
            {
                Assert.Contains("Anna Hansen", component.Markup);
            });

            Assert.IsTrue(component.Find("button.btn-update").HasAttribute("disabled"));
            Assert.IsTrue(component.Find("button.btn-delete").HasAttribute("disabled"));
        }

        [TestMethod]
        public void AdminStaff_CreateButtonIsDisabled_WhenNameIsMissing() {
            SetupSuccessfulInitialLoad();

            var component = Render<AdminStaff>();

            component.WaitForAssertion(() =>
            {
                Assert.Contains("Anna Hansen", component.Markup);
            });

            var createButton = component.Find("button.btn-create");

            Assert.IsTrue(createButton.HasAttribute("disabled"));
        }
    }
}
