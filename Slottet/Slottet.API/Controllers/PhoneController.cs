using Microsoft.AspNetCore.Mvc;
using Slottet.Application.Interfaces;
using Slottet.Shared;

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

        /// <summary>
        /// Gets all phones.
        /// </summary>
        /// <returns>Returns all phones.</returns>
        [HttpGet("Phones")]
        public async Task<ActionResult<IEnumerable<PhoneDTO>>> GetAllPhonesAsync()
        {
            var phones = await _phoneService.GetAllPhonesAsync();

            return Ok(phones);
        }

        /// <summary>
        /// Gets a phone by ID.
        /// </summary>
        /// <param name="id">The ID of the phone to retrieve.</param>
        /// <returns>Returns the phone if found, otherwise NotFound.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PhoneDTO>> GetPhoneByIdAsync(Guid id)
        {
            var phone = await _phoneService.GetPhoneByIdAsync(id);

            if (phone == null)
            {
                return NotFound();
            }

            return Ok(phone);
        }

        /// <summary>
        /// Updates a phone by ID.
        /// </summary>
        /// <param name="id">The ID of the phone to update.</param>
        /// <param name="dto">DTO object containing updated phone values.</param>
        /// <returns>Returns NoContent if the update succeeds, otherwise BadRequest or NotFound.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPhoneAsync(Guid id, PhoneDTO dto)
        {
            if (dto == null || id != dto.PhoneID)
            {
                return BadRequest();
            }

            var updated = await _phoneService.PutPhoneAsync(id, dto);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Creates a new phone.
        /// </summary>
        /// <param name="dto">DTO object containing phone information.</param>
        /// <returns>Returns the created phone.</returns>
        [HttpPost]
        public async Task<ActionResult<PhoneDTO>> PostPhoneAsync(PhoneDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.PhoneNumber) || dto.DepartmentID == Guid.Empty)
            {
                return BadRequest();
            }

            var createdPhone = await _phoneService.PostPhoneAsync(dto);
            if (createdPhone == null)
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(GetPhoneByIdAsync), new { id = createdPhone.PhoneID }, createdPhone);
        }

        /// <summary>
        /// Deletes a phone by ID.
        /// </summary>
        /// <param name="id">The ID of the phone to delete.</param>
        /// <returns>Returns NoContent if the deletion succeeds, otherwise NotFound.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoneAsync(Guid id)
        {
            var deleted = await _phoneService.DeletePhoneAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
