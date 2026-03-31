using Microsoft.AspNetCore.Mvc;
using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;

namespace Slottet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpecialResponsibilityController : Controller
    {
        private readonly IControllerRepository<SpecialResponsibility> _repository;

        public SpecialResponsibilityController(IControllerRepository<SpecialResponsibility> specialResponsibilityRepository) {
            _repository = specialResponsibilityRepository;
        }

        //Get: special responsibility
        [HttpGet("api/SpecialResponsibilities")]
        public async Task<ActionResult<IEnumerable<SpecialResponsibility>>> GetAll() {
            var specialResponsibility = await _repository.GetAllAsync();
            return Ok(specialResponsibility);
        }

        //Get: special responsibility by id
        [HttpGet("{id}")]
        public async Task<ActionResult<SpecialResponsibility>> GetById(Guid id) {
            var specialResponsibility = await _repository.GetByIdAsync(id);

            if (specialResponsibility == null) {
                return NotFound();
            }

            return Ok(specialResponsibility);
        }

        //Post: special responsibility
        [HttpPost]
        public async Task<ActionResult<Resident>> CreateSpecialResponsibility([FromBody] SpecialResponsibility specialResponsibility) {
            if (specialResponsibility == null) {
                return BadRequest();
            }

            var createdSpecialResponsibility = await _repository.CreateAsync(specialResponsibility);

            return CreatedAtAction(nameof(GetById), new { id = createdSpecialResponsibility.SpecialResponsibilityID }, createdSpecialResponsibility);
        }

        //Put: special responsibility by id
        [HttpPut("{id}")]
        public async Task<ActionResult<Resident>> UpdateSpecialResponsibility(Guid id, [FromBody] SpecialResponsibility specialResponsibility) {
            if (specialResponsibility == null || id != specialResponsibility.SpecialResponsibilityID) {
                return BadRequest();
            }

            var existingSpecialResponsibility = await _repository.GetByIdAsync(id);

            if (existingSpecialResponsibility == null) {
                return NotFound();
            }

            existingSpecialResponsibility.TaskName = specialResponsibility.TaskName;

            await _repository.UpdateAsync(specialResponsibility);

            return NoContent();
        }

        //Delete: special responsibility by id
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSpecialResponsibility(Guid id) {
            var existingSpecialResponsibility = await _repository.GetByIdAsync(id);

            if (existingSpecialResponsibility == null) {
                return NotFound();
            }

            await _repository.DeleteAsync(id);

            return NoContent();
        }
    }
}
