using Microsoft.AspNetCore.Mvc;
using Moq;
using Slottet.API.Controllers;
using Slottet.Application.Interfaces;
using Slottet.Shared;

namespace Slottet.API.Test {
    [TestClass]
    public class PhoneControllerTests {
        private Mock<IPhoneDTOService> _mockService = null!;
        private PhoneController _controller = null!;

        private static readonly Guid ValidPhoneId = Guid.NewGuid();
        private static readonly Guid ValidDepartmentId = Guid.NewGuid();

        [TestInitialize]
        public void Setup() {
            _mockService = new Mock<IPhoneDTOService>();
            _controller = new PhoneController(_mockService.Object);
        }

        private static PhoneDTO ValidDto() => new PhoneDTO {
            PhoneID = ValidPhoneId,
            PhoneNumber = "12345678",
            DepartmentID = ValidDepartmentId
        };

        /// <summary>
        /// Ensures the controller returns 200 OK when the service successfully retrieves a list of phones.
        /// Verifies that GetAllPhonesAsync wraps the service result in an OkObjectResult.
        /// </summary>
        [TestMethod]
        public async Task GetAllPhonesAsync_Returns200OK() {
            _mockService
                .Setup(p => p.GetAllPhonesAsync())
                .ReturnsAsync(new List<PhoneDTO>());

            var result = await _controller.GetAllPhonesAsync();

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        /// <summary>
        /// Ensures the controller returns all phones provided by the service.
        /// Verifies that GetAllPhonesAsync returns the phone collection supplied by IPhoneDTOService.
        /// </summary>
        [TestMethod]
        public async Task GetAllPhonesAsync_ReturnsAllPhonesFromService() {
            var phones = new List<PhoneDTO> {
                new PhoneDTO {
                    PhoneID = Guid.NewGuid(),
                    PhoneNumber = "23456789",
                    DepartmentID = Guid.NewGuid()
                },
                new PhoneDTO {
                    PhoneID = Guid.NewGuid(),
                    PhoneNumber = "34567890",
                    DepartmentID = Guid.NewGuid()
                }
            };

            _mockService
                .Setup(p => p.GetAllPhonesAsync())
                .ReturnsAsync(phones);

            var result = await _controller.GetAllPhonesAsync();

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var ok = (OkObjectResult)result.Result!;
            Assert.IsInstanceOfType(ok.Value, typeof(IEnumerable<PhoneDTO>));
            var items = (IEnumerable<PhoneDTO>)ok.Value!;
            Assert.AreEqual(2, items.Count());
        }

        /// <summary>
        /// Ensures 200 OK is returned when the requested phone exists.
        /// Verifies that GetPhoneByIdAsync returns OkObjectResult when the service resolves a DTO for the requested ID.
        /// </summary>
        [TestMethod]
        public async Task GetPhoneByIdAsync_Returns200OK_WhenFound() {
            _mockService
                .Setup(p => p.GetPhoneByIdAsync(ValidPhoneId))
                .ReturnsAsync(ValidDto());

            var result = await _controller.GetPhoneByIdAsync(ValidPhoneId);

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        /// <summary>
        /// Ensures the correct phone data is returned when the phone exists.
        /// Verifies that GetPhoneByIdAsync preserves the expected DTO values in the OkObjectResult payload.
        /// </summary>
        [TestMethod]
        public async Task GetPhoneByIdAsync_ReturnsCorrectPhone_WhenFound() {
            var dto = ValidDto();
            _mockService
                .Setup(p => p.GetPhoneByIdAsync(ValidPhoneId))
                .ReturnsAsync(dto);

            var result = await _controller.GetPhoneByIdAsync(ValidPhoneId);

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var ok = (OkObjectResult)result.Result!;
            var returned = (PhoneDTO)ok.Value!;
            Assert.AreEqual(ValidPhoneId, returned.PhoneID);
            Assert.AreEqual("12345678", returned.PhoneNumber);
            Assert.AreEqual(ValidDepartmentId, returned.DepartmentID);
        }

        /// <summary>
        /// Ensures 404 NotFound is returned when the requested phone does not exist.
        /// Verifies that GetPhoneByIdAsync maps a null service response to NotFoundResult.
        /// </summary>
        [TestMethod]
        public async Task GetPhoneByIdAsync_Returns404NotFound_WhenMissing() {
            _mockService
                .Setup(p => p.GetPhoneByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((PhoneDTO?)null);

            var result = await _controller.GetPhoneByIdAsync(Guid.NewGuid());

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        /// <summary>
        /// Ensures a successful creation returns 201 Created with correct route information.
        /// Verifies that PostPhoneAsync returns CreatedAtActionResult with status code, action name, 
        /// and route ID populated from the created DTO.
        /// </summary>
        [TestMethod]
        public async Task PostPhoneAsync_Returns201Created_WhenServiceSucceeds() {
            var dto = ValidDto();
            _mockService
                .Setup(p => p.PostPhoneAsync(It.IsAny<PhoneDTO>()))
                .ReturnsAsync(dto);

            var result = await _controller.PostPhoneAsync(dto);

            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            var created = (CreatedAtActionResult)result.Result!;
            Assert.AreEqual(201, created.StatusCode);
            Assert.AreEqual("GetPhoneById", created.ActionName);
            Assert.AreEqual(ValidPhoneId, created.RouteValues!["id"]);
        }

        /// <summary>
        /// Ensures 400 BadRequest is returned when the input DTO is null.
        /// Verifies that PostPhoneAsync rejects a null DTO with BadRequestResult before calling the service.
        /// </summary>
        [TestMethod]
        public async Task PostPhoneAsync_Returns400BadRequest_WhenDtoIsNull() {
            var result = await _controller.PostPhoneAsync(null!);

            Assert.IsInstanceOfType(result.Result, typeof(BadRequestResult));
        }

        /// <summary>
        /// Ensures validation fails when required fields are empty, resulting in 400 BadRequest.
        /// Verifies that PostPhoneAsync rejects an invalid DTO when required fields fail controller validation.
        /// </summary>
        [TestMethod]
        public async Task PostPhoneAsync_Returns400BadRequest_WhenRequiredFieldsAreMissing() {
            var invalidDto = ValidDto();
            invalidDto.PhoneNumber = "";

            var result = await _controller.PostPhoneAsync(invalidDto);

            Assert.IsInstanceOfType(result.Result, typeof(BadRequestResult));
        }

        /// <summary>
        /// Ensures validation fails when DepartmentID is missing, resulting in 400 BadRequest.
        /// Verifies that PostPhoneAsync rejects Guid.Empty as an invalid DepartmentID.
        /// </summary>
        [TestMethod]
        public async Task PostPhoneAsync_Returns400BadRequest_WhenDepartmentIsMissing() {
            var invalidDto = ValidDto();
            invalidDto.DepartmentID = Guid.Empty;

            var result = await _controller.PostPhoneAsync(invalidDto);

            Assert.IsInstanceOfType(result.Result, typeof(BadRequestResult));
        }

        /// <summary>
        /// Ensures 400 BadRequest is returned when the service fails to create a phone.
        /// Verifies that PostPhoneAsync maps a null create result from the service to BadRequestResult.
        /// </summary>
        [TestMethod]
        public async Task PostPhoneAsync_Returns400BadRequest_WhenServiceReturnsNull() {
            var dto = ValidDto();
            _mockService
                .Setup(s => s.PostPhoneAsync(It.IsAny<PhoneDTO>()))
                .ReturnsAsync((PhoneDTO?)null);

            var result = await _controller.PostPhoneAsync(dto);

            Assert.IsInstanceOfType(result.Result, typeof(BadRequestResult));
        }

        /// <summary>
        /// Ensures 204 NoContent is returned when an update operation succeeds.
        /// Verifies that PutPhoneAsync maps a successful service update result to NoContentResult.
        /// </summary>
        [TestMethod]
        public async Task PutPhoneAsync_Returns204NoContent_WhenUpdateSucceeds() {
            var dto = ValidDto();
            _mockService
                .Setup(p => p.PutPhoneAsync(ValidPhoneId, It.IsAny<PhoneDTO>()))
                .ReturnsAsync(true);

            var result = await _controller.PutPhoneAsync(ValidPhoneId, dto);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        /// <summary>
        /// Ensures 400 BadRequest is returned when the input DTO is null during update.
        /// Verifies that PutPhoneAsync rejects a null update DTO with BadRequestResult.
        /// </summary>
        [TestMethod]
        public async Task PutPhoneAsync_Returns400BadRequest_WhenDtoIsNull() {
            var result = await _controller.PutPhoneAsync(ValidPhoneId, null!);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        /// <summary>
        /// Ensures validation fails when required fields are missing during update.
        /// Verifies that PutPhoneAsync rejects an update DTO when required fields fail controller validation.
        /// </summary>
        [TestMethod]
        public async Task PutPhoneAsync_Returns400BadRequest_WhenRequiredFieldsAreMissing() {
            var invalidDto = ValidDto();
            invalidDto.PhoneNumber = "";

            var result = await _controller.PutPhoneAsync(ValidPhoneId, invalidDto);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        /// <summary>
        /// Ensures 404 NotFound is returned when attempting to update a non-existent phone.
        /// Verifies that PutPhoneAsync maps a false service update result for the requested phone ID to NotFoundResult.
        /// </summary>
        [TestMethod]
        public async Task PutPhoneAsync_Returns404NotFound_WhenPhoneMissing() {
            var dto = ValidDto();
            var missingId = Guid.NewGuid();
            dto.PhoneID = missingId;

            _mockService
                .Setup(s => s.PutPhoneAsync(missingId, dto))
                .ReturnsAsync(false);

            var result = await _controller.PutPhoneAsync(missingId, dto);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        /// <summary>
        /// Ensures 204 NoContent is returned when a delete operation succeeds.
        /// Verifies that DeletePhoneAsync maps a successful service delete result to NoContentResult.
        /// </summary>
        [TestMethod]
        public async Task DeletePhoneAsync_Returns204NoContent_WhenDeleteSucceeds() {
            _mockService
                .Setup(s => s.DeletePhoneAsync(ValidPhoneId))
                .ReturnsAsync(true);

            var result = await _controller.DeletePhoneAsync(ValidPhoneId);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        /// <summary>
        /// Ensures 404 NotFound is returned when attempting to delete a non-existent phone.
        /// Verifies that DeletePhoneAsync maps a false service delete result to NotFoundResult.
        /// </summary>
        [TestMethod]
        public async Task DeletePhoneAsync_Returns404NotFound_WhenPhoneMissing() {
            _mockService
                .Setup(s => s.DeletePhoneAsync(It.IsAny<Guid>()))
                .ReturnsAsync(false);

            var result = await _controller.DeletePhoneAsync(Guid.NewGuid());

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}
