using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Data;
using Slottet.Shared;

namespace Slottet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResidentController : Controller {
        private readonly IResidentDTOService _residentService;

        public ResidentController(IResidentDTOService residentService) {
            _residentService = residentService;
        }

        //Get: residents
        [HttpGet("Residents")]
        public async Task<ActionResult<IEnumerable<EditResidentDTO>>> GetAllAsync() {
            var residents = await _residentService.GetAllAsync();

            return Ok(residents);
        }

        //Get: resident by id
        [HttpGet("{id}")]
        public async Task<ActionResult<EditResidentDTO>> GetByIdAsync(Guid id) {
            var resident = await _residentService.GetByIdAsync(id);

            if(resident == null) {
                return NotFound();
            }

            return Ok(resident);
        }

        //Post: resident
        [HttpPost]
        public async Task<ActionResult<Resident>> CreateAsync([FromBody] EditResidentDTO dto) {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ResidentName) || dto.GroceryDayID == Guid.Empty) {
                return BadRequest();
            }

            var resident = await _residentService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetByIdAsync), new { id = resident.ResidentID }, resident);
        }

        //Put: resident by id
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] EditResidentDTO dto) {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ResidentName) || dto.GroceryDayID == Guid.Empty) {
                return BadRequest();
            }

            var updated = await _residentService.UpdateAsync(id, dto);

            if(!updated) {
                return NotFound();
            }

            return NoContent();
        }

        //Delete: resident by id
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id) {
            var deleted = await _residentService.DeleteAsync(id);

            if(!deleted) {
                return NotFound();
            }

            return NoContent();
        }
    }
}