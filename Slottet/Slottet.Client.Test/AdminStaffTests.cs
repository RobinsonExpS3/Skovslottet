using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Slottet.Client.Pages.AdminPages;
using Slottet.Shared;
using Slottet.Client.Test.Helpers;

namespace Slottet.Client.Test {
    /// <summary>
    /// bUnit tests for the AdminStaff page.
    /// Verifies rendering, API interaction, validation,
    /// authorization handling, and CRUD workflows.
    /// </summary>
    [TestClass]
    public class AdminStaffTests : BunitContext {
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
        /// Configures successful mock responses for all required
        /// API endpoints during component initialization.
        /// </summary>
        private void SetupSuccessfulInitialLoad() {
            _handler.AddStatus(HttpMethod.Get, "api/shiftboard/current", HttpStatusCode.OK);

            _handler.AddJson(HttpMethod.Get, "api/Department", new List<DepartmentLookupDTO> {
                new() { ID = Guid.NewGuid(), Name = "Slottet" }
            });

            _handler.AddJson(HttpMethod.Get, "api/Staff/Staffs", new List<EditStaffDTO> {
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

        /// <summary>
        /// Configures successful mock responses using a specific staff ID.
        /// Used for scenarios that require validating update or delete requests.
        /// </summary>
        private void SetupSuccessfulInitialLoadWithStaffId(Guid staffId) {
            _handler.AddStatus(HttpMethod.Get, "api/shiftboard/current", HttpStatusCode.OK);

            _handler.AddJson(HttpMethod.Get, "api/Department", new List<DepartmentLookupDTO> { 
                new() { ID = Guid.NewGuid(), Name = "Slottet" } 
            });

            _handler.AddJson(HttpMethod.Get, "api/Staff/Staffs", new List<EditStaffDTO> {
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
        /// Ensures staff members from the API are displayed when the initial load succeeds.
        /// Verifies that the rendered AdminStaff markup contains staff name and initials from the mocked API response.
        /// </summary>
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

        /// <summary>
        /// Ensures AdminStaff requests all required data sources during component initialization.
        /// Verifies that the bUnit render triggers GET calls for shiftboard access, departments, staff, and special responsibilities.
        /// </summary>
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

        /// <summary>
        /// Ensures access is denied when the shiftboard authorization check returns Forbidden.
        /// Verifies that the rendered component shows the access denied message for a 403 API response.
        /// </summary>
        [TestMethod]
        public void AdminStaff_ShowsAccessDeniedMessage_WhenShiftBoardReturnsForbidden() {
            _handler.AddStatus(HttpMethod.Get, "api/shiftboard/current", HttpStatusCode.Forbidden);
            _handler.AddJson(HttpMethod.Get, "api/Department", new List<DepartmentLookupDTO>());
            _handler.AddJson(HttpMethod.Get, "api/Staff/Staffs", new List<EditStaffDTO>());
            _handler.AddJson(HttpMethod.Get, "api/SpecialResponsibility/SpecialResponsibilities",
                new List<SpecialResponsibilityEntryDto>());

            var component = Render<AdminStaff>();

            component.WaitForAssertion(() => {
                Assert.Contains("Du har ikke adgang til denne side.", component.Markup);
            });
        }

        /// <summary>
        /// Ensures access is denied when the shiftboard authorization check returns Unauthorized.
        /// Verifies that the rendered component shows the access denied message for a 401 API response.
        /// </summary>
        [TestMethod]
        public void AdminStaff_ShowsAccessDeniedMessage_WhenShiftBoardReturnsUnauthorized() {
            _handler.AddStatus(HttpMethod.Get, "api/shiftboard/current", HttpStatusCode.Unauthorized);
            _handler.AddJson(HttpMethod.Get, "api/Department", new List<DepartmentLookupDTO>());
            _handler.AddJson(HttpMethod.Get, "api/Staff/Staffs", new List<EditStaffDTO>());
            _handler.AddJson(HttpMethod.Get, "api/SpecialResponsibility/SpecialResponsibilities",
                new List<SpecialResponsibilityEntryDto>());

            var component = Render<AdminStaff>();

            component.WaitForAssertion(() =>
            {
                Assert.Contains("Du har ikke adgang til denne side.", component.Markup);
            });
        }

        /// <summary>
        /// Ensures a department load failure is surfaced as a general load error.
        /// Verifies that a failed Department endpoint response renders the expected error message.
        /// </summary>
        [TestMethod]
        public void AdminStaff_ShowsLoadError_WhenDepartmentLoadFails() {
            _handler.AddStatus(HttpMethod.Get, "api/shiftboard/current", HttpStatusCode.OK);
            _handler.AddStatus(HttpMethod.Get, "api/Department", HttpStatusCode.InternalServerError);

            _handler.AddJson(HttpMethod.Get, "api/Staff/Staffs", new List<EditStaffDTO>());
            _handler.AddJson(HttpMethod.Get, "api/SpecialResponsibility/SpecialResponsibilities",
                new List<SpecialResponsibilityEntryDto>());

            var component = Render<AdminStaff>();

            component.WaitForAssertion(() =>
            {
                Assert.Contains("Kunne ikke loade data", component.Markup);
            });
        }

        /// <summary>
        /// Ensures a staff load failure is surfaced as a general load error.
        /// Verifies that a failed Staff endpoint response renders the expected error message.
        /// </summary>
        [TestMethod]
        public void AdminStaff_ShowsStaffLoadError_WhenStaffLoadFails() {
            _handler.AddStatus(HttpMethod.Get, "api/shiftboard/current", HttpStatusCode.OK);

            _handler.AddJson(HttpMethod.Get, "api/Department", new List<DepartmentLookupDTO> { 
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

        /// <summary>
        /// Ensures valid create form input submits both staff and login creation requests.
        /// Verifies that bUnit input/change events followed by the create click trigger POST calls to Staff and Auth endpoints.
        /// </summary>
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

            // Fill create form with valid values.
            component.Find("input[placeholder='Navn']").Input("Peter Jensen");
            component.Find("input[placeholder='Initialer']").Input("PJ");
            component.Find("select").Change("staff");

            // Trigger create action.
            component.Find("button.btn-create").Click();

            component.WaitForAssertion(() =>
            {
                Assert.IsTrue(WasCalled(HttpMethod.Post, "api/Staff"));
                Assert.IsTrue(WasCalled(HttpMethod.Post, "api/Auth/createUserForStaff"));
            });
        }

        /// <summary>
        /// Ensures staff creation errors stop the login creation flow.
        /// Verifies that a failed Staff POST renders the create error message and does not call the Auth endpoint.
        /// </summary>
        [TestMethod]
        public void AdminStaff_ShowsError_WhenCreateStaffFails() {
            SetupSuccessfulInitialLoad();

            _handler.AddStatus(HttpMethod.Post, "api/Staff", HttpStatusCode.InternalServerError);

            var component = Render<AdminStaff>();

            component.WaitForAssertion(() =>
            {
                Assert.Contains("Anna Hansen", component.Markup);
            });

            // Fill create form with valid values.
            component.Find("input[placeholder='Navn']").Input("Peter Jensen");
            component.Find("input[placeholder='Initialer']").Input("PJ");
            component.Find("select").Change("staff");

            // Attempt to create staff member.
            component.Find("button.btn-create").Click();

            component.WaitForAssertion(() =>
            {
                Assert.Contains("Kunne ikke oprette medarbejder", component.Markup);

                // Verify login creation endpoint was not called.
                Assert.IsFalse(WasCalled(HttpMethod.Post, "api/Auth/createUserForStaff"));
            });
        }

        /// <summary>
        /// Ensures login creation failure is handled after staff creation succeeds.
        /// Verifies that a successful Staff POST followed by a failed Auth POST renders the partial create error message.
        /// </summary>
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

            // Fill create form with valid values.
            component.Find("input[placeholder='Navn']").Input("Peter Jensen");
            component.Find("input[placeholder='Initialer']").Input("PJ");
            component.Find("select").Change("staff");

            // Trigger create workflow.
            component.Find("button.btn-create").Click();

            component.WaitForAssertion(() =>
            {
                Assert.Contains("Medarbejderen blev oprettet, men login kunne ikke oprettes.", component.Markup);
            });
        }

        /// <summary>
        /// Ensures selecting a staff row binds the selected staff values into the edit form.
        /// Verifies that a bUnit row click populates the form inputs and enables update and delete actions.
        /// </summary>
        [TestMethod]
        public void AdminStaff_SelectStaff_PopulatesFormAndEnablesUpdateAndDelete() {
            SetupSuccessfulInitialLoad();

            var component = Render<AdminStaff>();

            component.WaitForAssertion(() =>
            {
                Assert.Contains("Anna Hansen", component.Markup);
            });

            // Select staff row from rendered table.
            component.Find("tr.staff-row").Click();

            var nameInput = component.Find("input[placeholder='Navn']");
            var initialsInput = component.Find("input[placeholder='Initialer']");

            // Verify form fields are populated with selected staff values.
            Assert.AreEqual("Anna Hansen", nameInput.GetAttribute("value"));
            Assert.AreEqual("AH", initialsInput.GetAttribute("value"));

            // Verify update and delete buttons become enabled.
            Assert.IsFalse(component.Find("button.btn-update").HasAttribute("disabled"));
            Assert.IsFalse(component.Find("button.btn-delete").HasAttribute("disabled"));
        }

        /// <summary>
        /// Ensures deleting a selected staff member calls the API with the selected staff ID.
        /// Verifies that selecting a rendered staff row and clicking delete triggers the expected DELETE request.
        /// </summary>
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

            // Select staff row and trigger delete action.
            component.Find("tr.staff-row").Click();
            component.Find("button.btn-delete").Click();

            component.WaitForAssertion(() =>
            {
                Assert.IsTrue(WasCalled(HttpMethod.Delete, $"api/Staff/{staffId}"));
            });
        }

        /// <summary>
        /// Ensures update and delete actions are unavailable before a staff row is selected.
        /// Verifies that the rendered action buttons have the disabled attribute when no staff member is selected.
        /// </summary>
        [TestMethod]
        public void AdminStaff_UpdateAndDeleteButtonsAreDisabled_WhenNoStaffIsSelected() {
            SetupSuccessfulInitialLoad();

            var component = Render<AdminStaff>();

            component.WaitForAssertion(() =>
            {
                Assert.Contains("Anna Hansen", component.Markup);
            });

            // Verify action buttons are initially disabled.
            Assert.IsTrue(component.Find("button.btn-update").HasAttribute("disabled"));
            Assert.IsTrue(component.Find("button.btn-delete").HasAttribute("disabled"));
        }

        /// <summary>
        /// Ensures staff creation cannot be submitted while required form input is missing.
        /// Verifies that the rendered create button is disabled before a valid name is entered.
        /// </summary>
        [TestMethod]
        public void AdminStaff_CreateButtonIsDisabled_WhenNameIsMissing() {
            SetupSuccessfulInitialLoad();

            var component = Render<AdminStaff>();

            component.WaitForAssertion(() =>
            {
                Assert.Contains("Anna Hansen", component.Markup);
            });

            var createButton = component.Find("button.btn-create");

            // Verify create button remains disabled without required input.
            Assert.IsTrue(createButton.HasAttribute("disabled"));
        }
    }
}
