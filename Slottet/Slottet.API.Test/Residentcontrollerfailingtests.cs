using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Slottet.API.Controllers;
using Slottet.Application.Interfaces;
using Slottet.Shared;

namespace Slottet.Tests.Controllers
{
    [TestClass]
    public class ResidentControllerFailingTests
    {
        private Mock<IResidentDTOService> _mockService = null!;
        private ResidentController _controller = null!;

        private static readonly Guid ValidResidentId = Guid.NewGuid();
        private static readonly Guid ValidGroceryDayId = Guid.NewGuid();

        [TestInitialize]
        public void Setup()
        {
            _mockService = new Mock<IResidentDTOService>();
            _controller = new ResidentController(_mockService.Object);
        }

        private static EditResidentDTO ValidDto() => new EditResidentDTO
        {
            ResidentID = ValidResidentId,
            ResidentName = "Jane",
            GroceryDayID = ValidGroceryDayId
        };

        // Now correctly asserts 200 OK
        [TestMethod]
        public async Task GetAllResidentsAsync_Returns200OK()
        {
            _mockService
                .Setup(s => s.GetAllResidentsAsync())
                .ReturnsAsync(new List<EditResidentDTO>());

            var result = await _controller.GetAllResidentsAsync();

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult)); 
        }

        // Now correctly asserts count of 2
        [TestMethod]
        public async Task GetAllResidentsAsync_ReturnsCorrectCount()
        {
            var residents = new List<EditResidentDTO>
            {
                new EditResidentDTO { ResidentID = Guid.NewGuid(), ResidentName = "Jane" },
                new EditResidentDTO { ResidentID = Guid.NewGuid(), ResidentName = "John" }
            };

            _mockService
                .Setup(s => s.GetAllResidentsAsync())
                .ReturnsAsync(residents);

            var result = await _controller.GetAllResidentsAsync();

            var ok = (OkObjectResult)result.Result!;
            var items = (IEnumerable<EditResidentDTO>)ok.Value!;

            Assert.AreEqual(2, items.Count()); 
        }

        // Now correctly asserts the actual ValidResidentId
        [TestMethod]
        public async Task GetResidentByIdAsync_ReturnsCorrectId()
        {
            _mockService
                .Setup(s => s.GetResidentByIdAsync(ValidResidentId))
                .ReturnsAsync(ValidDto());

            var result = await _controller.GetResidentByIdAsync(ValidResidentId);
            var ok = (OkObjectResult)result.Result!;
            var returned = (EditResidentDTO)ok.Value!;

            Assert.AreEqual(ValidResidentId, returned.ResidentID); 
        }

        // Now correctly asserts 201 CreatedAtActionResult
        [TestMethod]
        public async Task PostResidentAsync_Returns201Created_OnValidDto()
        {
            var dto = ValidDto();
            _mockService
                .Setup(s => s.PostResidentAsync(It.IsAny<EditResidentDTO>()))
                .ReturnsAsync(dto);

            var result = await _controller.PostResidentAsync(dto);

            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult)); 
        }
    }
}