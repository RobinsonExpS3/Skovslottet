using Microsoft.AspNetCore.Mvc;
using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;

namespace Slottet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhoneController : ControllerBase
    {
        private readonly IPhoneDTOService _phoneService;

        public PhoneController(IPhoneDTOService phoneService)
        {
            _phoneService = phoneService;
        }

        [HttpGet("Phones")]
        public async Task<ActionResult<IEnumerable<Phone>>> GetAllAsync()
        {
            var phones = await _phoneService.GetAllAsync();

            return Ok(phones);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Phone>> GetPhone(Guid id)
        {
            var phone = await _phoneService.GetByIdAsync(id);

            if (phone == null)
            {
                return NotFound();
            }

            return Ok(phone);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPhone(Guid id, Phone phone)
        {
            if (id != phone.PhoneID)
            {
                return BadRequest();
            }

            var updated = await _phoneService.UpdateAsync(id, phone);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Phone>> PostPhone(Phone phone)
        {
            var createdPhone = await _phoneService.CreateAsync(phone);

            return CreatedAtAction(nameof(GetPhone), new { id = createdPhone.PhoneID }, createdPhone);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _phoneService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
