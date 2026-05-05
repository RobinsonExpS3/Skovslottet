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

        //---------------------------
        //Get Api/resident/Residents
        //---------------------------


        // --------------------------- Returns200OK ----------------------------------
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
        // --------------------------- ReturnsAllResidentsFromService ----------------------------------
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
        // ---------------------------------------------------------------------
        // GET api/resident/{id}
        // ---------------------------------------------------------------------

        // --------------------------- Returns200OK_WhenFound ----------------------------------
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

        // ---------------------------ReturnsCorrectResident_WhenFound ---------------------------
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

        //--------------------------- Returns404NotFound_WhenMissing ----------------------------------
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

        // ---------------------------------------------------------------------
        // POST api/resident/
        // ---------------------------------------------------------------------

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
            Assert.AreEqual(nameof(_controller.GetResidentByIdAsync), created.ActionName);
        }

        [TestMethod]
        public async Task PostResidentAsync_ReturnsBadRequest_WhenDtoIsNull()
        {
            //Act
            var result = await _controller.PostResidentAsync(null!);
            //Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestResult));
        }

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

        // ---------------------------------------------------------------------
        // PUT api/resident/{id}
        // ---------------------------------------------------------------------

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

        // ---------------------------------------------------------------------
        // DELETE api/resident/{id}
        // ---------------------------------------------------------------------

        [TestMethod]
        public async Task DeleteResidentAsync_Returns204NoContent_WhenDeleteSucceeds()
        {
            _mockService
                .Setup(s => s.DeleteResidentAsync(ValidResidentId))
                .ReturnsAsync(true);

            var result = await _controller.DeleteResidentAsync(ValidResidentId);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

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