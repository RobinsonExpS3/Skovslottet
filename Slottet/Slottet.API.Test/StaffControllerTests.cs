using Microsoft.AspNetCore.Mvc;
using Moq;
using Slottet.API.Controllers;
using Slottet.Application.Interfaces;
using Slottet.Shared;

namespace Slottet.API.Test
{
    [TestClass]
    public class StaffControllerTests
    {
        private Mock<IStaffDTOService> _mockService = null!;
        private StaffController _controller = null!;

        private static readonly Guid ValidStaffId = Guid.NewGuid();
        private static readonly Guid ValidDepartmentId = Guid.NewGuid();

        [TestInitialize]
        public void Setup()
        {
            _mockService = new Mock<IStaffDTOService>();
            _controller = new StaffController(_mockService.Object);
        }

        private static EditStaffDTO ValidDto() => new EditStaffDTO
        {
            StaffID = ValidStaffId,
            StaffName = "Anna Andersen",
            Initials = "AA",
            Role = "Pædagog",
            DepartmentID = ValidDepartmentId
        };

        /// <summary>
        /// Ensures the controller returns 200 OK when the service successfully retrieves staff.
        /// </summary>
        [TestMethod]
        public async Task GetAllStaffAsync_Returns200OK()
        {
            _mockService
                .Setup(s => s.GetAllStaffAsync())
                .ReturnsAsync(new List<EditStaffDTO>());

            var result = await _controller.GetAllStaffAsync();

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        /// <summary>
        /// Ensures the controller returns all staff provided by the service.
        /// </summary>
        [TestMethod]
        public async Task GetAllStaffAsync_ReturnsAllStaffFromService()
        {
            var staff = new List<EditStaffDTO>
            {
                new EditStaffDTO
                {
                    StaffID = Guid.NewGuid(),
                    StaffName = "Anna Andersen",
                    Initials = "AA",
                    Role = "Pædagog",
                    DepartmentID = Guid.NewGuid()
                },
                new EditStaffDTO
                {
                    StaffID = Guid.NewGuid(),
                    StaffName = "Bo Berg",
                    Initials = "BB",
                    Role = "Pædagog",
                    DepartmentID = Guid.NewGuid()
                }
            };

            _mockService
                .Setup(s => s.GetAllStaffAsync())
                .ReturnsAsync(staff);

            var result = await _controller.GetAllStaffAsync();

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var ok = (OkObjectResult)result.Result!;
            Assert.IsInstanceOfType(ok.Value, typeof(IEnumerable<EditStaffDTO>));
            var items = (IEnumerable<EditStaffDTO>)ok.Value!;
            Assert.AreEqual(2, items.Count());
        }

        /// <summary>
        /// Ensures 200 OK is returned when the requested staff member exists.
        /// </summary>
        [TestMethod]
        public async Task GetStaffByIdAsync_Returns200OK_WhenFound()
        {
            _mockService
                .Setup(s => s.GetStaffByIdAsync(ValidStaffId))
                .ReturnsAsync(ValidDto());

            var result = await _controller.GetStaffByIdAsync(ValidStaffId);

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        /// <summary>
        /// Ensures the correct staff data is returned when the staff member exists.
        /// </summary>
        [TestMethod]
        public async Task GetStaffByIdAsync_ReturnsCorrectStaff_WhenFound()
        {
            var dto = ValidDto();
            _mockService
                .Setup(s => s.GetStaffByIdAsync(ValidStaffId))
                .ReturnsAsync(dto);

            var result = await _controller.GetStaffByIdAsync(ValidStaffId);

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var ok = (OkObjectResult)result.Result!;
            var returned = (EditStaffDTO)ok.Value!;
            Assert.AreEqual(ValidStaffId, returned.StaffID);
            Assert.AreEqual("Anna Andersen", returned.StaffName);
            Assert.AreEqual("AA", returned.Initials);
        }

        /// <summary>
        /// Ensures 404 NotFound is returned when the requested staff member does not exist.
        /// </summary>
        [TestMethod]
        public async Task GetStaffByIdAsync_Returns404NotFound_WhenMissing()
        {
            _mockService
                .Setup(s => s.GetStaffByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((EditStaffDTO?)null);

            var result = await _controller.GetStaffByIdAsync(Guid.NewGuid());

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        /// <summary>
        /// Ensures a successful creation returns 201 Created with correct route information.
        /// </summary>
        [TestMethod]
        public async Task PostStaffAsync_Returns201Created_WhenServiceSucceeds()
        {
            var dto = ValidDto();
            _mockService
                .Setup(s => s.PostStaffAsync(It.IsAny<EditStaffDTO>()))
                .ReturnsAsync(dto);

            var result = await _controller.PostStaffAsync(dto);

            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            var created = (CreatedAtActionResult)result.Result!;
            Assert.AreEqual(201, created.StatusCode);
            Assert.AreEqual(nameof(_controller.GetStaffByIdAsync), created.ActionName);
            Assert.AreEqual(ValidStaffId, created.RouteValues!["id"]);
        }

        /// <summary>
        /// Ensures 400 BadRequest is returned when the input DTO is null.
        /// </summary>
        [TestMethod]
        public async Task PostStaffAsync_Returns400BadRequest_WhenDtoIsNull()
        {
            var result = await _controller.PostStaffAsync(null!);

            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        /// <summary>
        /// Ensures validation fails when required fields are empty, resulting in 400 BadRequest.
        /// </summary>
        [TestMethod]
        public async Task PostStaffAsync_Returns400BadRequest_WhenRequiredFieldsAreMissing()
        {
            var invalidDto = ValidDto();
            invalidDto.StaffName = "";

            var result = await _controller.PostStaffAsync(invalidDto);

            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        /// <summary>
        /// Ensures validation fails when DepartmentID is missing, resulting in 400 BadRequest.
        /// </summary>
        [TestMethod]
        public async Task PostStaffAsync_Returns400BadRequest_WhenDepartmentIsMissing()
        {
            var invalidDto = ValidDto();
            invalidDto.DepartmentID = Guid.Empty;

            var result = await _controller.PostStaffAsync(invalidDto);

            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        /// <summary>
        /// Ensures 400 BadRequest is returned when the service fails to create the staff member.
        /// </summary>
        [TestMethod]
        public async Task PostStaffAsync_Returns400BadRequest_WhenServiceReturnsNull()
        {
            var dto = ValidDto();
            _mockService
                .Setup(s => s.PostStaffAsync(It.IsAny<EditStaffDTO>()))
                .ReturnsAsync((EditStaffDTO?)null);

            var result = await _controller.PostStaffAsync(dto);

            Assert.IsInstanceOfType(result.Result, typeof(BadRequestResult));
        }

        /// <summary>
        /// Ensures 204 NoContent is returned when an update operation succeeds.
        /// </summary>
        [TestMethod]
        public async Task PutStaffAsync_Returns204NoContent_WhenUpdateSucceeds()
        {
            var dto = ValidDto();
            _mockService
                .Setup(s => s.PutStaffAsync(ValidStaffId, It.IsAny<EditStaffDTO>()))
                .ReturnsAsync(true);

            var result = await _controller.PutStaffAsync(ValidStaffId, dto);

            Assert.IsInstanceOfType(result.Result, typeof(NoContentResult));
        }

        /// <summary>
        /// Ensures 400 BadRequest is returned when the input DTO is null during update.
        /// </summary>
        [TestMethod]
        public async Task PutStaffAsync_Returns400BadRequest_WhenDtoIsNull()
        {
            var result = await _controller.PutStaffAsync(ValidStaffId, null!);

            Assert.IsInstanceOfType(result.Result, typeof(BadRequestResult));
        }

        /// <summary>
        /// Ensures validation fails when required fields are missing during update.
        /// </summary>
        [TestMethod]
        public async Task PutStaffAsync_Returns400BadRequest_WhenRequiredFieldsAreMissing()
        {
            var invalidDto = ValidDto();
            invalidDto.Role = "";

            var result = await _controller.PutStaffAsync(ValidStaffId, invalidDto);

            Assert.IsInstanceOfType(result.Result, typeof(BadRequestResult));
        }

        /// <summary>
        /// Ensures 404 NotFound is returned when attempting to update a non-existent staff member.
        /// </summary>
        [TestMethod]
        public async Task PutStaffAsync_Returns404NotFound_WhenStaffMissing()
        {
            var dto = ValidDto();
            _mockService
                .Setup(s => s.PutStaffAsync(It.IsAny<Guid>(), dto))
                .ReturnsAsync(false);

            var result = await _controller.PutStaffAsync(Guid.NewGuid(), dto);

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        /// <summary>
        /// Ensures 204 NoContent is returned when a delete operation succeeds.
        /// </summary>
        [TestMethod]
        public async Task DeleteStaffAsync_Returns204NoContent_WhenDeleteSucceeds()
        {
            _mockService
                .Setup(s => s.DeleteStaffAsync(ValidStaffId))
                .ReturnsAsync(true);

            var result = await _controller.DeleteStaffAsync(ValidStaffId);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        /// <summary>
        /// Ensures 404 NotFound is returned when attempting to delete a non-existent staff member.
        /// </summary>
        [TestMethod]
        public async Task DeleteStaffAsync_Returns404NotFound_WhenStaffMissing()
        {
            _mockService
                .Setup(s => s.DeleteStaffAsync(It.IsAny<Guid>()))
                .ReturnsAsync(false);

            var result = await _controller.DeleteStaffAsync(Guid.NewGuid());

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}