using Microsoft.AspNetCore.Mvc;
using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;

namespace Slottet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResidentController : Controller
    {
        private readonly IBaseRepository<Resident> _repository;

        public ResidentController(IBaseRepository<Resident> residentRepository)
        {
            _repository = residentRepository;
        }

        //Get: residents
        [HttpGet("api/Residents")]
        public async Task<ActionResult<IEnumerable<Resident>>> GetAll()
        {
            var residents = await _repository.GetAllAsync();
            return Ok(residents);
        }

        //Get: resident by id
        [HttpGet("{id}")]
        public async Task<ActionResult<Resident>> GetById(Guid id)
        {
            var resident = await _repository.GetByIdAsync(id);

            if (resident == null)
            {
                return NotFound();
            }

            return Ok(resident);
        }

        //Post: resident
        [HttpPost]
        public async Task<ActionResult<Resident>> CreateResident([FromBody] Resident resident)
        {
            if (resident == null)
            {
                return BadRequest();
            }

            await _repository.AddAsync(resident);

            return CreatedAtAction(nameof(GetById), new { id = resident.ResidentID }, resident);
        }

        //Put: resident by id
        [HttpPut("{id}")]
        public async Task<ActionResult<Resident>> UpdateResident(Guid id, [FromBody] Resident resident)
        {
            if (resident == null || id != resident.ResidentID)
            {
                return BadRequest();
            }

            var existingResident = await _repository.GetByIdAsync(id);

            if (existingResident == null)
            {
                return NotFound();
            }

            existingResident.ResidentName = resident.ResidentName;
            existingResident.GroceryDay = resident.GroceryDay;
            existingResident.IsActive = resident.IsActive;

            await _repository.UpdateAsync(existingResident);

            return NoContent();
        }

        //Delete: resident by id
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteResident(Guid id)
        {
            var existingResident = await _repository.GetByIdAsync(id);

            if (existingResident == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(id);

            return NoContent();
        }
    }
}
