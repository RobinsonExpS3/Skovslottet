using Microsoft.AspNetCore.Mvc;
using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;

namespace Slottet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShiftboardController : Controller
    {
        private readonly IBaseRepository<ShiftBoard> _repository;

        public ShiftboardController(IBaseRepository<ShiftBoard> repository) {
            _repository = repository;
        }

        [HttpGet("ShiftBoards")]
        public async Task<ActionResult<IEnumerable<ShiftBoard>>> GetAll() {
            var shiftBoard = await _repository.GetAllAsync();
            return Ok(shiftBoard);
        }
    }
}
