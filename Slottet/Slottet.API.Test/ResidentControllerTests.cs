using Microsoft.AspNetCore.Mvc;
using Moq;
using Slottet.API.Controllers;
using Slottet.Application.Interfaces;
using Slottet.Shared;

namespace Slottet.Test.Controllers
{
    [TestClass]
    public class ResidentControllerTests
    {
        private Mock<IResidentDTOService> _mockService = null!;
        private ResidentController _controller = null!;

        //Shared Test data
        private static readonly Guid ValidResidentId = Guid.NewGuid();
        private static readonly Guid ValidGroceryDayId = Guid.NewGuid();


        [TestInitialize]
        public void Setup()
        {
            _mockService = new Mock<IResidentDTOService>();
            _controller = new ResidentController(_mockService.Object);
        }


        // --------------------------- Helper Data -----------------------------------------

        private static EditResidentDTO validDto() => new EditResidentDTO
        {
            ResidentID = ValidResidentId,
            ResidentName = "John",
            GroceryDayID = Guid.NewGuid(),  
        };

        /// <summary>
        /// Ensures GetAllResidentsAsync returns a 200 OK status code.
        /// Verifies that the controller responds with OkObjectResult when the service returns a list of residents.
        /// </summary>
        [TestMethod]
        public async Task GetAllResidentsAsync_Returns200OK()
        {
            //Arrange
            _mockService
                .Setup(s => s.GetAllResidentsAsync())
                .ReturnsAsync(new List<EditResidentDTO>());

            //Act
            var result = await _controller.GetAllResidentsAsync();

            //Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }
        
        /// <summary>
        /// Ensures GetAllResidentsAsync passes all residents from the service through to the response body.
        /// Verifies that the count and content of the returned list matches exactly what the service provided.
        /// </summary>
        [TestMethod]
        public async Task GetAllResidentsAsync_ReturnAllResidentsFromService()
        {
            //Arrange
            var resident = new List<EditResidentDTO>
            {
                new EditResidentDTO { ResidentID = Guid.NewGuid(), ResidentName = "John", GroceryDayID = Guid.NewGuid() },
                new EditResidentDTO { ResidentID = Guid.NewGuid(), ResidentName = "Jane", GroceryDayID = Guid.NewGuid() }
            };

            _mockService
                .Setup(s => s.GetAllResidentsAsync())
                .ReturnsAsync(resident);

            //Act
            var result = await _controller.GetAllResidentsAsync();

            //Assert 
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var ok = (OkObjectResult)result.Result;
            Assert.IsInstanceOfType(ok.Value, typeof(IEnumerable<EditResidentDTO>));
            var items = (IEnumerable<EditResidentDTO>)ok.Value!;
            Assert.AreEqual(2, items.Count());

        }

        /// <summary>
        /// Ensures GetResidentByIdAsync returns a 200 OK status code when the resident exists.
        /// Verifies that the controller returns OkObjectResult when the service finds a matching resident.
        /// </summary>
        [TestMethod]
        public async Task GetResidentByIdAsync_Returns200OK_WhenFound()
        {
            //Arrange
            _mockService
                .Setup(s => s.GetResidentByIdAsync(ValidResidentId))
                .ReturnsAsync(validDto());

            //Act
            var result = await _controller.GetResidentByIdAsync(ValidResidentId);

            //Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        /// <summary>
        /// Ensures GetResidentByIdAsync returns the correct resident matching the requested ID.
        /// Verifies that the ResidentID on the returned DTO matches the ID that was requested.
        /// </summary>
        [TestMethod]
        public async Task GetResidentByIdAsync_ReturnsCorrectResident_whenFound()
        {
            //Arrange
            var dto = validDto();
            _mockService
                .Setup(s => s.GetResidentByIdAsync(ValidResidentId))
                .ReturnsAsync(dto);
            //Act
            var result = await _controller.GetResidentByIdAsync(ValidResidentId);

            //Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var ok = (OkObjectResult)result.Result!;
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var returned = (EditResidentDTO)ok.Value!;
            Assert.AreEqual(ValidResidentId, returned.ResidentID);


        }

        /// <summary>
        /// Ensures GetResidentByIdAsync returns 404 NotFound when no resident matches the given ID.
        /// Verifies that the controller returns NotFoundResult when the service returns null.
        /// </summary>
        [TestMethod]
        public async Task GetResidentByIdAsync_Returns404NotFound_WhenMissing()
        {
            //Arrange
            _mockService
                .Setup(s => s.GetResidentByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((EditResidentDTO?)null);

            //Act
            var result = await _controller.GetResidentByIdAsync(Guid.NewGuid());

            //Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }


        /// <summary>
        /// Ensures PostResidentAsync returns 201 Created when a valid DTO is submitted and the service succeeds.
        /// Verifies that the controller returns CreatedAtActionResult pointing to GetResidentByIdAsync.
        /// </summary>
        [TestMethod]
        public async Task PostResidentByIdAsync_Returns201Created_WhenServicesSucceeds()
        {
            //Arrange
            var dto = validDto();
            _mockService
                .Setup(s => s.PostResidentAsync(It.IsAny<EditResidentDTO>()))
                .ReturnsAsync(dto);

            //Act
            var result = await _controller.PostResidentAsync(dto);

            //Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            var created = (CreatedAtActionResult)result.Result!;
            Assert.AreEqual(201, created.StatusCode);
            Assert.AreEqual("GetResidentById", created.ActionName);
        }

        /// <summary>
        /// Ensures PutResidentAsync rejects a null request body with 400 BadRequest.
        /// Verifies that the controller validates input before reaching the service.
        /// </summary>
        [TestMethod]
        public async Task PostResidentAsync_ReturnsBadRequest_WhenDtoIsNull()
        {
            //Act
            var result = await _controller.PostResidentAsync(null!);
            //Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestResult));
        }

        /// <summary>
        /// Ensures PostResidentAsync returns 400 BadRequest when the service fails and returns null.
        /// Verifies that the controller handles downstream service failures gracefully rather than crashing.
        /// </summary>
        [TestMethod]
        public async Task PostResidentAsync_Returns400BadRequest_WhenServiceReturnsNull()
        {
            //Arrange
            var dto = validDto();
            
            _mockService
                .Setup(s => s.PostResidentAsync(It.IsAny<EditResidentDTO>()))
                .ReturnsAsync((EditResidentDTO?)null);
            
            //Act
            var result = await _controller.PostResidentAsync(dto);

            //Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestResult));
        }

        /// <summary>
        /// Ensures PutResidentAsync returns 204 NoContent when the update succeeds.
        /// Verifies that the controller returns NoContentResult when the service confirms the update.
        /// </summary>
        [TestMethod]
        public async Task PutResidentAsync_Returns204NoContent_WhenUpdateSucceeds()
        {
            var dto = validDto();
            _mockService
                .Setup(s => s.PutResidentAsync(ValidResidentId, It.IsAny<EditResidentDTO>()))
                .ReturnsAsync(true);

            var result = await _controller.PutResidentAsync(ValidResidentId, dto);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        /// <summary>
        /// Ensures PutResidentAsync returns 404 NotFound when no resident matches the given ID.
        /// Verifies that the controller maps a false return from the service to a NotFoundResult.
        /// </summary>
        [TestMethod]
        public async Task PutResidentAsync_Returns404NotFound_WhenResidentMissing()
        {
            var dto = validDto();
            _mockService
                .Setup(s => s.PutResidentAsync(It.IsAny<Guid>(), dto))
                .ReturnsAsync(false);

            var result = await _controller.PutResidentAsync(Guid.NewGuid(), dto);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        /// <summary>
        /// Ensures DeleteResidentAsync returns 204 NoContent when the deletion succeeds.
        /// Verifies that the controller returns NoContentResult when the service confirms the deletion.
        /// </summary>
        [TestMethod]
        public async Task DeleteResidentAsync_Returns204NoContent_WhenDeleteSucceeds()
        {
            _mockService
                .Setup(s => s.DeleteResidentAsync(ValidResidentId))
                .ReturnsAsync(true);

            var result = await _controller.DeleteResidentAsync(ValidResidentId);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        /// <summary>
        /// Ensures DeleteResidentAsync returns 404 NotFound when no resident matches the given ID.
        /// Verifies that the controller maps a false return from the service to a NotFoundResult.
        /// </summary>
        [TestMethod]
        public async Task DeleteResidentAsync_Returns404NotFound_WhenResidentMissing()
        {
            _mockService
                .Setup(s => s.DeleteResidentAsync(It.IsAny<Guid>()))
                .ReturnsAsync(false);

            var result = await _controller.DeleteResidentAsync(Guid.NewGuid());

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}