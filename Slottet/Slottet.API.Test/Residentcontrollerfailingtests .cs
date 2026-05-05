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


        // ─────────────────────────────────────────────────────────────────────
        // FAIL 1 — Wrong status code expected
        // We assert 404 NotFound, but the controller returns 200 OK.
        // Error in Test Explorer:
        //   Assert.IsInstanceOfType failed.
        //   Expected: NotFoundResult
        //   Actual:   OkObjectResult
        // ─────────────────────────────────────────────────────────────────────
        [TestMethod]
        public async Task FAILING_GetAllResidentsAsync_WrongStatusCode()
        {
            _mockService
                .Setup(s => s.GetAllResidentsAsync())
                .ReturnsAsync(new List<EditResidentDTO>());
            //Act
            var result = await _controller.GetAllResidentsAsync();

            //Assert: controller returns OkObjectResult, not NotFoundResult
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        // ─────────────────────────────────────────────────────────────────────
        // FAIL 2 — Wrong count expected
        // The service returns 2 residents but we assert there are 5.
        // Error in Test Explorer:
        //   Assert.AreEqual failed.
        //   Expected: 5
        //   Actual:   2
        // ─────────────────────────────────────────────────────────────────────
        [TestMethod]
        public async Task FAILING_GetAllResidentsAsync_WrongCount()
        {
            //Arrange: service returns 2 residents
            var residents = new List<EditResidentDTO>
            {
                new EditResidentDTO { ResidentID = Guid.NewGuid(), ResidentName = "Jane", },
                new EditResidentDTO { ResidentID = Guid.NewGuid(), ResidentName = "John", } 
            };

            _mockService
                .Setup(s => s.GetAllResidentsAsync())
                .ReturnsAsync(residents);
            //Act
            var result = await _controller.GetAllResidentsAsync();

            var ok = (OkObjectResult)result.Result!;
            var items = (IEnumerable<EditResidentDTO>)ok.Value!;

            //Assert: service only returned 2, not 5
            Assert.AreEqual(5, items.Count());
        }

        // ─────────────────────────────────────────────────────────────────────
        // FAIL 3 — Wrong ID expected
        // We ask for ValidResidentId but assert that a different ID came back.
        // Error in Test Explorer:
        //   Assert.AreEqual failed.
        //   Expected: <some-other-guid>
        //   Actual:   <ValidResidentId>
        // ─────────────────────────────────────────────────────────────────────
        [TestMethod]
        public async Task FAILING_GetResidentByIdAsync_WrongIdReturned()
        {
            _mockService
                .Setup(s => s.GetResidentByIdAsync(ValidResidentId))
                .ReturnsAsync(ValidDto());

            //Act
            var result = await _controller.GetResidentByIdAsync(ValidResidentId);
            var ok = (OkObjectResult)result.Result!;
            var returned = (EditResidentDTO)ok.Value!;

            //Assert: we're comparing against a brand new random Guid
            Assert.AreEqual(Guid.NewGuid(), returned.ResidentID);
        }

        // ─────────────────────────────────────────────────────────────────────
        // FAIL 4 — Expecting BadRequest but controller returns Created
        // We pass a valid DTO and the service succeeds, but we assert 400.
        // Error in Test Explorer:
        //   Assert.IsInstanceOfType failed.
        //   Expected: BadRequestResult
        //   Actual:   CreatedAtActionResult
        // ─────────────────────────────────────────────────────────────────────
        [TestMethod]
        public async Task FAILING_PostResidentAsync_ExpectsBadRequestOnValidDto()
        {
            //Arrange: valid DTO and service returns it successfully
            var dto = ValidDto();
            _mockService
                .Setup(s => s.PostResidentAsync(It.IsAny<EditResidentDTO>()))
                .ReturnsAsync(dto);
            //Act
            var result = await _controller.PostResidentAsync(dto);

            //Assert: a valid DTO succeeds, so controller returns 201 Created
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestResult));
        }
    }
}