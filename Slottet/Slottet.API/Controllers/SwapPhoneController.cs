using Microsoft.AspNetCore.Mvc;
using Slottet.Application.Interfaces;
using Slottet.Shared;

namespace Slottet.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SwapPhoneController : ControllerBase
    {
        private readonly ISwapPhoneDTOService _swapPhoneService;

        public SwapPhoneController(ISwapPhoneDTOService swapPhoneService)
        {
            _swapPhoneService = swapPhoneService;
        }

        /// <summary>
        /// Gets all phones with their latest staff assignment.
        /// </summary>
        /// <returns>Returns all phone assignments.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SwapPhoneDTO>>> GetAllSwapPhonesAsync()
        {
            return Ok(await _swapPhoneService.GetAllSwapPhonesAsync());
        }
        /// <summary>
        /// Assigns a phone to a staff member.
        /// </summary>
        /// <param name="dto">DTO object containing phone and staff information.</param>
        /// <returns>Returns NoContent if the assignment succeeds, otherwise NotFound.</returns>
        [HttpPost("assign")]
        public async Task<IActionResult> PostSwapPhoneAsync([FromBody] SwapPhoneDTO dto)
        {
            //if (dto == null ||  dto.StaffID == Guid.Empty) return BadRequest();

            var updated = await _swapPhoneService.PostSwapPhoneAsync(dto);
            if (!updated) return NotFound();

            return NoContent();
        }
    }
}
